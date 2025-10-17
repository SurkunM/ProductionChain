using Microsoft.AspNetCore.SignalR;
using ProductionChain.Contracts.IHubs;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Hubs;

public class TaskQueueNotificationHub : Hub<ITaskQueueNotificationHub>
{
    private readonly string _managerRole = RolesEnum.Manager.ToString();

    public override async Task OnConnectedAsync()
    {
        if (Context.User?.Identity?.IsAuthenticated == true && Context.User.IsInRole(_managerRole))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _managerRole);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User?.IsInRole(_managerRole) == true)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _managerRole);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
