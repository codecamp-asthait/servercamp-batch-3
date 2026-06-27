using Dukaan.Notification.Domain.Entities;

namespace Dukaan.Notification.Application.Interfaces;

public interface INotificationDispatcher
{
    string ChannelType { get; }
    Task DispatchAsync(NotificationEntity notification, string? customerEmail, string? rawData, CancellationToken ct);
}
