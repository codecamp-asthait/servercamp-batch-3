using Dukaan.Notification.Application.Core.Abstractions;
using Dukaan.Notification.Application.Features.Notifications.Dtos;
using ErrorOr;

namespace Dukaan.Notification.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(
    int Page = 1,
    int PageSize = 20,
    bool UnreadOnly = false)
    : IQuery<ErrorOr<NotificationListDto>>
{
    public int Skip => (Page - 1) * PageSize;
}
