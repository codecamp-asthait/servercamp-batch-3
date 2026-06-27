using Dukaan.Notification.Application.Core.Abstractions;
using Dukaan.Notification.Application.Features.Notifications.Dtos;
using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Domain.Enums;
using ErrorOr;
using NotificationEntity = Dukaan.Notification.Domain.Entities.NotificationEntity;

namespace Dukaan.Notification.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler(
    IRepository<NotificationEntity> notificationRepository)
    : IQueryHandler<GetNotificationsQuery, ErrorOr<NotificationListDto>>
{
    public async Task<ErrorOr<NotificationListDto>> Handle(
        GetNotificationsQuery query, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.FindAsync(
            n => n.ChannelType == NotificationChannelType.InApp && (!query.UnreadOnly || !n.IsRead),
            cancellationToken: cancellationToken);

        var ordered = notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        var totalCount = ordered.Count;

        var items = ordered
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(n => new NotificationDto(
                n.Id,
                n.EventType,
                n.OrderId,
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt))
            .ToList();

        return new NotificationListDto(items, totalCount, query.Page, query.PageSize);
    }
}
