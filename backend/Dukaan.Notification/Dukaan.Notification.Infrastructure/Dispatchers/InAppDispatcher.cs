using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Domain.Enums;
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

    public async Task DispatchAsync(NotificationEventData eventData, CancellationToken ct)
    {
        var orderDisplayId = eventData.OrderDisplayId ?? eventData.OrderId?.ToString("N")[..8] ?? "N/A";

        string title, message;
        if (EventTemplates.TryGetValue(eventData.EventType, out var template))
        {
            title = string.Format(template.Title, orderDisplayId);
            message = string.Format(template.MessageTemplate, orderDisplayId);
        }
        else
        {
            title = $"Order {eventData.EventType}";
            message = $"Your order (event: {eventData.EventType}) has been updated.";
        }

        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = eventData.CustomerId,
            TenantId = eventData.TenantId,
            EventType = eventData.EventType,
            OrderId = eventData.OrderId,
            ChannelType = NotificationChannelType.InApp,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();
        await repository.AddAsync(notification, ct);
        await repository.SaveChangesAsync(ct);

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

        await hubContext.Clients.Group($"user-{eventData.CustomerId}")
            .SendAsync("Notification", dto, ct);

        logger.LogInformation(
            "In-app notification persisted and pushed for Customer={CustomerId}, NotificationId={NotificationId}",
            eventData.CustomerId, notification.Id);
    }
}
