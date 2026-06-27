using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Infrastructure.Dispatchers;

public class InAppDispatcher(
    IServiceScopeFactory scopeFactory,
    IHubContext<NotificationHub> hubContext,
    ILogger<InAppDispatcher> logger) : INotificationDispatcher
{
    public string ChannelType => "in-app";

    private static readonly Dictionary<string, (string Title, string MessageTemplate)> EventTemplates = new()
    {
        ["order-placed"]    = ("Order Placed",    "Your order #{0} has been placed successfully."),
        ["order-confirmed"] = ("Order Confirmed", "Your order #{0} has been confirmed."),
        ["order-shipped"]   = ("Order Shipped",   "Your order #{0} has been shipped."),
        ["order-delivered"] = ("Order Delivered", "Your order #{0} has been delivered."),
        ["order-cancelled"] = ("Order Cancelled", "Your order #{0} has been cancelled."),
    };

    public async Task DispatchAsync(NotificationEntity notification, string? customerEmail, string? rawData, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();

        notification.IsRead = false;
        notification.CreatedAt = DateTime.UtcNow;

        await repository.AddAsync(notification, ct);
        await repository.SaveChangesAsync(ct);

        var orderDisplayId = notification.OrderId?.ToString("N")[..8] ?? "N/A";

        string title, message;
        if (EventTemplates.TryGetValue(notification.EventType, out var template))
        {
            title = string.Format(template.Title, orderDisplayId);
            message = string.Format(template.MessageTemplate, orderDisplayId);
        }
        else
        {
            title = $"Order {notification.EventType}";
            message = $"Your order (event: {notification.EventType}) has been updated.";
        }

        var dto = new
        {
            id = notification.Id.ToString(),
            eventType = notification.EventType,
            orderId = notification.OrderId?.ToString(),
            title,
            message,
            isRead = notification.IsRead,
            createdAt = notification.CreatedAt
        };

        await hubContext.Clients.Group($"user-{notification.CustomerId}")
            .SendAsync("Notification", dto, ct);

        logger.LogInformation(
            "In-app notification persisted and pushed for Customer={CustomerId}, NotificationId={NotificationId}",
            notification.CustomerId, notification.Id);
    }
}
