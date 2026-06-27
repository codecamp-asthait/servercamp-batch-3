using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dukaan.Notification.Infrastructure.Dispatchers;

public class NotificationDispatchManager(
    IEnumerable<INotificationDispatcher> dispatchers,
    ILogger<NotificationDispatchManager> logger) : INotificationDispatchManager
{
    public async Task DispatchAsync(
        NotificationEntity notification,
        IReadOnlyCollection<string> channels,
        string? customerEmail,
        string? rawData,
        CancellationToken ct)
    {
        var targetDispatchers = dispatchers.Where(d => channels.Contains(d.ChannelType));

        foreach (var dispatcher in targetDispatchers)
        {
            try
            {
                await dispatcher.DispatchAsync(notification, customerEmail, rawData, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Dispatcher {ChannelType} failed for NotificationId={NotificationId}",
                    dispatcher.ChannelType, notification.Id);
            }
        }
    }
}
