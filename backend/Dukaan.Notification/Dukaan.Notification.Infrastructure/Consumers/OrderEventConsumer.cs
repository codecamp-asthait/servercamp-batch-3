using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Dukaan.Notification.Infrastructure.Consumers;

public class OrderEventConsumer : BackgroundService
{
    private const int TrimIntervalBatches = 10;
    private const int TrimSafetyBuffer = 100;

    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderEventConsumer> _logger;
    private int _processedCount;

    private const string StreamName = "order-events";
    private const string GroupName = "notification-group";

    public OrderEventConsumer(
        IConnectionMultiplexer redis,
        IServiceScopeFactory scopeFactory,
        ILogger<OrderEventConsumer> logger)
    {
        _redis = redis;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var consumerName = $"consumer-{Environment.MachineName}-{Guid.NewGuid():N}";

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
                var entries = await db.StreamReadGroupAsync(
                    StreamName,
                    GroupName,
                    consumerName,
                    ">",
                    count: 10);

                foreach (var entry in entries)
                {
                    await ProcessEntryAsync(entry, cancellationToken);
                    await db.StreamAcknowledgeAsync(StreamName, GroupName, entry.Id);
                }

                if (entries.Length > 0 && ++_processedCount % TrimIntervalBatches == 0)
                {
                    await TrimStreamAsync(db);
                }

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

    private async Task ProcessEntryAsync(StreamEntry entry, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dispatchManager = scope.ServiceProvider.GetRequiredService<INotificationDispatchManager>();

        var fields = entry.Values.ToDictionary(v => v.Name.ToString(), v => v.Value.ToString());

        var eventType = fields.GetValueOrDefault("event", "unknown");
        var customerId = Guid.Parse(fields.GetValueOrDefault("customer_id", Guid.Empty.ToString()));
        var tenantId = Guid.Parse(fields.GetValueOrDefault("tenant_id", Guid.Empty.ToString()));
        var orderId = fields.TryGetValue("order_id", out var oid) && Guid.TryParse(oid, out var parsedOrderId)
            ? parsedOrderId : (Guid?)null;
        var orderDisplayId = fields.GetValueOrDefault("order_display_id");
        var customerEmail = fields.GetValueOrDefault("customer_email");
        var rawData = fields.GetValueOrDefault("data");

        var notificationTypes = fields.GetValueOrDefault("notification_types", "in-app")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var eventData = new NotificationEventData(
            eventType, customerId, tenantId, orderId, orderDisplayId, customerEmail, rawData);

        _logger.LogInformation(
            "Notification received: Customer={CustomerId}, Event={EventType}",
            customerId, eventType);

        await dispatchManager.DispatchAsync(eventData, notificationTypes, ct);
    }

    private async Task TrimStreamAsync(IDatabase db)
    {
        try
        {
            var pendingInfo = await db.StreamPendingAsync(StreamName, GroupName);

            if (pendingInfo.PendingMessageCount == 0)
            {
                var trimmed = await db.StreamTrimAsync(StreamName, TrimSafetyBuffer);
                if (trimmed > 0)
                {
                    _logger.LogInformation("Trimmed {Count} acknowledged entries from stream '{StreamName}'", trimmed, StreamName);
                }
            }
            else
            {
                var oldestPending = await db.StreamRangeAsync(StreamName, "-", "+", count: 1, Order.Ascending);
                if (oldestPending.Length > 0)
                {
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

    private async Task ReclaimPendingAsync(IDatabase db, string consumerName)
    {
        try
        {
            var pendingInfo = await db.StreamPendingAsync(StreamName, GroupName);
            if (pendingInfo.PendingMessageCount == 0) return;

            var consumers = pendingInfo.Consumers;
            if (consumers == null || consumers.Length == 0) return;

            var staleConsumer = consumers.FirstOrDefault(c => c.Name != consumerName);
            if (staleConsumer.Name == default) return;

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
