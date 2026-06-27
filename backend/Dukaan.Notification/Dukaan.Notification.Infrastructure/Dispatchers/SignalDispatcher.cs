using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Dukaan.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Infrastructure.Dispatchers;

public class SignalDispatcher(
    IHubContext<NotificationHub> hubContext,
    ILogger<SignalDispatcher> logger) : INotificationDispatcher
{
    public string ChannelType => "signal";

    public async Task DispatchAsync(NotificationEventData eventData, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(eventData.RawData))
        {
            logger.LogWarning("Skipping signal for Customer={CustomerId}: rawData is empty", eventData.CustomerId);
            return;
        }

        await hubContext.Clients.Group($"user-{eventData.CustomerId}")
            .SendAsync("Signal", eventData.RawData, ct);

        logger.LogInformation(
            "Signal pushed for Customer={CustomerId}, EventType={EventType}",
            eventData.CustomerId, eventData.EventType);
    }
}
