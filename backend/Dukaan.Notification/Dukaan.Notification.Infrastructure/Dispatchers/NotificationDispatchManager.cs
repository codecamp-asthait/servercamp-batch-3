using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Models;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Infrastructure.Dispatchers;

public class NotificationDispatchManager(
    IEnumerable<INotificationDispatcher> dispatchers,
    ILogger<NotificationDispatchManager> logger) : INotificationDispatchManager
{
    public async Task DispatchAsync(
        NotificationEventData eventData,
        IReadOnlyCollection<string> channels,
        CancellationToken ct)
    {
        var targetDispatchers = dispatchers.Where(d => channels.Contains(d.ChannelType));

        foreach (var dispatcher in targetDispatchers)
        {
            try
            {
                await dispatcher.DispatchAsync(eventData, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Dispatcher {ChannelType} failed for Customer={CustomerId}, Event={EventType}",
                    dispatcher.ChannelType, eventData.CustomerId, eventData.EventType);
            }
        }
    }
}
