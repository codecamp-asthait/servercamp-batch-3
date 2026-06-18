using ErrorOr;
using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Application.Core.Abstractions;
using NotificationEntity = Dukaan.Notification.Domain.Entities.NotificationEntity;

namespace Dukaan.Notification.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkAsReadHandler(
    IRepository<NotificationEntity> notificationRepository)
    : ICommandHandler<MarkAsReadCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        MarkAsReadCommand command, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.FindFirstAsync(
            n => n.Id == command.NotificationId,
            trackChanges: true,
            cancellationToken: cancellationToken);

        if (notification is null)
            return NotificationErrors.NotFound;

        notification.IsRead = true;
        await notificationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
