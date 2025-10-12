using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IHubs;

namespace ProductionChain.BusinessLogic.Hubs;

public class TaskQueueAlertHub : Hub<ITaskQueueAlertHub>
{
    private readonly ILogger<TaskQueueAlertHub> _logger;

    public TaskQueueAlertHub(ILogger<TaskQueueAlertHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task TaskQueueAlert(string employee)
    {
        await Clients.All.TaskQueueAlert(employee);
    }
}
