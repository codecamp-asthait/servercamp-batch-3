using Dukaan.Notification.Application.Models;

namespace Dukaan.Notification.Application.Interfaces;

public interface INotificationDispatcher
{
    string ChannelType { get; }
    Task DispatchAsync(NotificationEventData eventData, CancellationToken ct);
}
