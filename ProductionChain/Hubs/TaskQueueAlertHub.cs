using Microsoft.AspNetCore.SignalR;
using ProductionChain.Contracts.IHubs;

namespace ProductionChain.Hubs;

public class TaskQueueAlertHub : Hub<ITaskQueueAlertHub>
{
    public async Task TaskQueueAlert(string employee)
    {
        await Clients.All.TaskQueueAlert(employee);
    }
}
