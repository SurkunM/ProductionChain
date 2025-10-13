using Microsoft.AspNetCore.SignalR;
using ProductionChain.Contracts.IHubs;

namespace ProductionChain.BusinessLogic.Hubs;

public class TaskQueueAlertHub : Hub<ITaskQueueAlertHub>
{
    public async Task SendMessage(string employee)
    {
        await Clients.All.TaskQueueAlert(employee);
    }
}
