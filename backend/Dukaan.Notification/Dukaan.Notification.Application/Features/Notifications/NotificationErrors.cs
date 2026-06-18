using ErrorOr;

namespace Dukaan.Notification.Application.Features.Notifications;

public static class NotificationErrors
{
    public static Error NotFound => Error.NotFound(
        "Notification.NotFound", "Notification not found.");
    public static Error NotOwned => Error.Forbidden(
        "Notification.NotOwned", "Notification does not belong to the current user.");
}
