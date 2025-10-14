using Microsoft.AspNetCore.SignalR;
using ProductionChain.Contracts.IHubs;

namespace ProductionChain.Hubs;

public class TaskQueueAlertHub : Hub<ITaskQueueNotificationHub>
{
    public async Task TaskQueueAlert(string employee)
    {
        await Clients.All.NotifyManagers(employee);
    }
}
