using Dukaan.Notification.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Notification.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId)
    : ICommand<ErrorOr<Success>>;
