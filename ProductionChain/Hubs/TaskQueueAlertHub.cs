using Microsoft.AspNetCore.SignalR;

namespace ProductionChain.Hubs;

public class TaskQueueAlertHub : Hub
{
    public async Task TaskQueueAlert()
    {
        await Clients.All.SendAsync("taskQueueAlert");
    }
}
