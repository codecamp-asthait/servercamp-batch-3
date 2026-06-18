using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dukaan.Notification.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var customerId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(customerId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{customerId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var customerId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(customerId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{customerId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
