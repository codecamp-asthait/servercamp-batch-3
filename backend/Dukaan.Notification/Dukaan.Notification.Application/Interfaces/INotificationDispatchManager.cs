using Dukaan.Notification.Application.Models;

namespace Dukaan.Notification.Application.Interfaces;

public interface INotificationDispatchManager
{
    Task DispatchAsync(
        NotificationEventData eventData,
        IReadOnlyCollection<string> channels,
        CancellationToken ct);
}
