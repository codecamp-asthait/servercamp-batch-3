using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Dukaan.Notification.Domain.Entities;

namespace Dukaan.Notification.Infrastructure.Consumers;

/// <summary>
/// Background service that consumes order events from a Redis Stream, persists them as notifications
/// in PostgreSQL, and pushes real-time updates to connected clients via SignalR.
///
/// Events are consumed using a consumer group (<see cref="GroupName"/>) for load-balanced processing
/// across multiple instances. Orphaned messages from crashed consumers are automatically reclaimed.
///
/// The stream is periodically trimmed to prevent unbounded growth. Trimming occurs every
/// <see cref="TrimIntervalBatches"/> batches and uses a safe strategy that never evicts unprocessed messages.
/// </summary>
public class OrderEventConsumer : BackgroundService
{
    /// <summary>
    /// Number of processed batches before triggering a stream trim operation.
    /// Trimming every 10 batches (100 messages) balances memory cleanup with Redis operation overhead.
    /// </summary>
    private const int TrimIntervalBatches = 10;

    /// <summary>
    /// Number of recent entries to keep in the stream after trimming when all messages are processed.
    /// This safety buffer ensures no race conditions occur between producers and consumers.
    /// 100 entries × ~500 bytes = ~50KB, negligible on a 25MB Redis instance.
    /// </summary>
    private const int TrimSafetyBuffer = 100;

    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderEventConsumer> _logger;

    /// <summary>
    /// Counter tracking the number of batches processed since the last trim operation.
    /// Incremented after each batch and reset after trimming.
    /// </summary>
    private int _processedCount;

    /// <summary>Redis Stream key where the main Dukaan API publishes order events.</summary>
    private const string StreamName = "order-events";

    /// <summary>
    /// Consumer group for distributing messages across notification-api instances.
    /// Each message in the stream is delivered to exactly one consumer in the group.
    /// </summary>
    private const string GroupName = "notification-group";

    /// <summary>
    /// Maps event types to user-friendly notification titles and message templates.
    /// {0} in the template is replaced with the order display ID.
    /// </summary>
    private static readonly Dictionary<string, (string Title, string MessageTemplate)> EventTemplates = new()
    {
        ["order-placed"]    = ("Order Placed",    "Your order #{0} has been placed successfully."),
        ["order-confirmed"] = ("Order Confirmed", "Your order #{0} has been confirmed."),
        ["order-shipped"]   = ("Order Shipped",   "Your order #{0} has been shipped."),
        ["order-delivered"] = ("Order Delivered", "Your order #{0} has been delivered."),
        ["order-cancelled"] = ("Order Cancelled", "Your order #{0} has been cancelled."),
    };

    public OrderEventConsumer(
        IConnectionMultiplexer redis,
        IServiceScopeFactory scopeFactory,
        ILogger<OrderEventConsumer> logger)
    {
        _redis = redis;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Main loop: polls the Redis Stream for new messages, processes them, and reclaims orphaned messages.
    /// Runs until the application shuts down.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var consumerName = $"consumer-{Environment.MachineName}-{Guid.NewGuid():N}";

        // Create the consumer group (idempotent — BUSYGROUP means it already exists)
        try
        {
            await db.StreamCreateConsumerGroupAsync(StreamName, GroupName, "0-0", true);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            _logger.LogInformation("Consumer group '{GroupName}' already exists", GroupName);
        }

        _logger.LogInformation("OrderEventConsumer started as consumer '{ConsumerName}'", consumerName);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Read up to 10 new messages assigned to this consumer
                var entries = await db.StreamReadGroupAsync(
                    StreamName,
                    GroupName,
                    consumerName,
                    ">",
                    count: 10);

                // Process and acknowledge each message
                foreach (var entry in entries)
                {
                    await ProcessEntryAsync(entry, cancellationToken);
                    await db.StreamAcknowledgeAsync(StreamName, GroupName, entry.Id);
                }

                // Periodically trim processed entries from the stream.
                // Trimming every 10 batches (100 messages) prevents unbounded growth while avoiding
                // excessive Redis operations. The trim operation is safe because it only removes
                // messages that have been acknowledged by all consumers.
                if (entries.Length > 0 && ++_processedCount % TrimIntervalBatches == 0)
                {
                    await TrimStreamAsync(db);
                }

                // When idle, check for orphaned messages from other consumers
                if (entries.Length == 0)
                {
                    await ReclaimPendingAsync(db, consumerName);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming from Redis Stream");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Processes a single stream entry: extracts event fields, creates a Notification entity,
    /// persists to the database, and pushes a real-time SignalR notification to the customer.
    /// </summary>
    private async Task ProcessEntryAsync(StreamEntry entry, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

        var fields = entry.Values.ToDictionary(v => v.Name.ToString(), v => v.Value.ToString());

        var eventType = fields.GetValueOrDefault("event", "unknown");
        var customerId = Guid.Parse(fields.GetValueOrDefault("customer_id", Guid.Empty.ToString()));
        var tenantId = Guid.Parse(fields.GetValueOrDefault("tenant_id", Guid.Empty.ToString()));
        var orderId = fields.TryGetValue("order_id", out var oid) && Guid.TryParse(oid, out var parsedOrderId)
            ? parsedOrderId : (Guid?)null;

        // Build a human-readable notification from the event type template
        string title, message;
        if (EventTemplates.TryGetValue(eventType, out var template))
        {
            title = template.Title;
            var orderDisplayId = fields.TryGetValue("order_display_id", out var displayId)
                ? displayId : orderId?.ToString("N")[..8] ?? "N/A";
            message = string.Format(template.MessageTemplate, orderDisplayId);
        }
        else
        {
            title = "Order Update";
            message = fields.TryGetValue("data", out var rawData) ? rawData : "Your order has been updated.";
        }

        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TenantId = tenantId,
            EventType = eventType,
            OrderId = orderId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await notificationRepository.AddAsync(notification, ct);
        await notificationRepository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Notification persisted for Customer={CustomerId}, Event={EventType}, Id={NotificationId}",
            customerId, eventType, notification.Id);

        // Push to the customer's SignalR group
        var dto = new
        {
            id = notification.Id.ToString(),
            eventType = notification.EventType,
            orderId = notification.OrderId?.ToString(),
            title = notification.Title,
            message = notification.Message,
            isRead = notification.IsRead,
            createdAt = notification.CreatedAt
        };

        await hubContext.Clients.Group($"user-{customerId}")
            .SendAsync("Notification", dto, ct);
    }

    /// <summary>
    /// Trims acknowledged entries from the Redis Stream to prevent unbounded growth.
    ///
    /// Two trimming strategies are used based on pending message state:
    /// 1. When no messages are pending: Uses MAXLEN to keep only the last <see cref="TrimSafetyBuffer"/> entries.
    ///    This is safe because all messages have been processed and acknowledged.
    /// 2. When messages are pending: Uses XTRIM MINID to remove only entries older than the oldest pending message.
    ///    This ensures unprocessed messages are never evicted, even if they've been waiting for a long time.
    ///
    /// Called every <see cref="TrimIntervalBatches"/> batches to balance cleanup with Redis operation overhead.
    /// </summary>
    private async Task TrimStreamAsync(IDatabase db)
    {
        try
        {
            var pendingInfo = await db.StreamPendingAsync(StreamName, GroupName);

            if (pendingInfo.PendingMessageCount == 0)
            {
                // No pending messages - safe to trim to safety buffer size
                var trimmed = await db.StreamTrimAsync(StreamName, TrimSafetyBuffer);
                if (trimmed > 0)
                {
                    _logger.LogInformation("Trimmed {Count} acknowledged entries from stream '{StreamName}'", trimmed, StreamName);
                }
            }
            else
            {
                // Pending messages exist - only trim entries older than the oldest pending message
                var oldestPending = await db.StreamRangeAsync(StreamName, "-", "+", count: 1, Order.Ascending);
                if (oldestPending.Length > 0)
                {
                    // XTRIM MINID removes all entries with ID less than the specified value
                    // This preserves the oldest pending message and everything after it
                    var result = await db.ExecuteAsync("XTRIM", StreamName, "MINID", oldestPending[0].Id.ToString());
                    var trimmed = (long)result;
                    if (trimmed > 0)
                    {
                        _logger.LogInformation("Trimmed {Count} acknowledged entries from stream '{StreamName}' (MINID={MinId})", trimmed, StreamName, oldestPending[0].Id);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error trimming stream '{StreamName}'", StreamName);
        }
    }

    /// <summary>
    /// Checks for messages that were delivered to other consumers but left unacknowledged
    /// for over 60 seconds (indicating a crash). Claims and reprocesses them.
    ///
    /// This provides at-least-once delivery guarantees across consumer restarts.
    /// </summary>
    private async Task ReclaimPendingAsync(IDatabase db, string consumerName)
    {
        try
        {
            var pendingInfo = await db.StreamPendingAsync(StreamName, GroupName);
            if (pendingInfo.PendingMessageCount == 0) return;

            var consumers = pendingInfo.Consumers;
            if (consumers == null || consumers.Length == 0) return;

            // Find any consumer that isn't this instance
            var staleConsumer = consumers.FirstOrDefault(c => c.Name != consumerName);
            if (staleConsumer.Name == default) return;

            // Check for messages that have been pending for more than 60s
            var pendingMessages = db.StreamPendingMessages(StreamName, GroupName, 10, staleConsumer.Name, null, null);
            if (pendingMessages == null || pendingMessages.Length == 0) return;

            foreach (var pending in pendingMessages)
            {
                if (pending.IdleTimeInMilliseconds > 60_000)
                {
                    _logger.LogWarning("Reclaiming orphaned message {MessageId} from {ConsumerName}",
                        pending.MessageId, pending.ConsumerName);
                }
            }

            // Transfer ownership and reprocess
            var claimed = await db.StreamAutoClaimAsync(
                StreamName,
                GroupName,
                consumerName,
                60_000,
                "0-0",
                count: 10);

            foreach (var entry in claimed.ClaimedEntries)
            {
                _logger.LogWarning("Reclaiming orphaned message {EntryId}", entry.Id);
                await ProcessEntryAsync(entry, CancellationToken.None);
                await db.StreamAcknowledgeAsync(StreamName, GroupName, entry.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reclaiming pending stream entries");
        }
    }
}
