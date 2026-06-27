using Dukaan.Notification.Application.Core.Abstractions;
using Dukaan.Notification.Application.Features.Notifications.Dtos;
using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Enums;
using ErrorOr;
using NotificationEntity = Dukaan.Notification.Domain.Entities.NotificationEntity;

namespace Dukaan.Notification.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountHandler(
    IRepository<NotificationEntity> notificationRepository)
    : IQueryHandler<GetUnreadCountQuery, ErrorOr<UnreadCountDto>>
{
    public async Task<ErrorOr<UnreadCountDto>> Handle(
        GetUnreadCountQuery query, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.FindAsync(
            n => n.ChannelType == NotificationChannelType.InApp && !n.IsRead, cancellationToken: cancellationToken);

        return new UnreadCountDto(notifications.Count());
    }
}
