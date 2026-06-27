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

    public async Task DispatchAsync(NotificationEntity notification, string? customerEmail, string? rawData, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();

        notification.IsRead = false;
        notification.CreatedAt = DateTime.UtcNow;

        await repository.AddAsync(notification, ct);
        await repository.SaveChangesAsync(ct);

        var dto = new
        {
            id = notification.Id.ToString(),
            eventType = notification.EventType,
            orderId = notification.OrderId?.ToString(),
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
