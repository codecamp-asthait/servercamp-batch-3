using Dukaan.Notification.Domain.Entities;

namespace Dukaan.Notification.Application.Interfaces;

public interface INotificationDispatchManager
{
    Task DispatchAsync(
        NotificationEntity notification,
        IReadOnlyCollection<string> channels,
        string? customerEmail,
        string? rawData,
        CancellationToken ct);
}
