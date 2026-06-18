using Dukaan.Notification.Application.Features.Notifications.Commands.MarkAsRead;
using Dukaan.Notification.Application.Features.Notifications.Dtos;
using Dukaan.Notification.Application.Features.Notifications.Queries.GetNotifications;
using Dukaan.Notification.Application.Features.Notifications.Queries.GetUnreadCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Notification.Host.Controllers;

[Authorize]
public class NotificationsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<NotificationListDto>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool unreadOnly = false)
        => ToActionResult(await Mediator.Send(new GetNotificationsQuery(page, pageSize, unreadOnly)));

    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadCountDto>> GetUnreadCount()
        => ToActionResult(await Mediator.Send(new GetUnreadCountQuery()));

    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
        => ToActionResult(await Mediator.Send(new MarkAsReadCommand(id)));
}
