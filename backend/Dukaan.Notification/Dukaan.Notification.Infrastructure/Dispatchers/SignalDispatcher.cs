using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Infrastructure.Dispatchers;

public class SignalDispatcher(
    IHubContext<NotificationHub> hubContext,
    ILogger<SignalDispatcher> logger) : INotificationDispatcher
{
    public string ChannelType => "signal";

    public async Task DispatchAsync(NotificationEntity notification, string? customerEmail, string? rawData, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(rawData))
        {
            logger.LogWarning("Skipping signal for Customer={CustomerId}: rawData is empty", notification.CustomerId);
            return;
        }

        await hubContext.Clients.Group($"user-{notification.CustomerId}")
            .SendAsync("Signal", rawData, ct);

        logger.LogInformation(
            "Signal pushed for Customer={CustomerId}, EventType={EventType}",
            notification.CustomerId, notification.EventType);
    }
}
