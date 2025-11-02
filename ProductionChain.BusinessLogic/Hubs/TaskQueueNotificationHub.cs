using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ProductionChain.Contracts.IHubs;
using ProductionChain.Model.Enums;
using System.Security.Claims;

namespace ProductionChain.BusinessLogic.Hubs;

[Authorize]
public class TaskQueueNotificationHub : Hub<ITaskQueueNotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        var accountId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(role))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{role}_{accountId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var accountId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{role}_{accountId}");

        await base.OnDisconnectedAsync(exception);
    }
}
