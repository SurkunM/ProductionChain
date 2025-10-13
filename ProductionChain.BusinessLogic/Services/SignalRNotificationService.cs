using Microsoft.AspNetCore.SignalR;
using ProductionChain.BusinessLogic.Hubs;
using ProductionChain.Contracts.IHubs;
using ProductionChain.Contracts.IServices;

namespace ProductionChain.BusinessLogic.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<TaskQueueAlertHub, ITaskQueueAlertHub> _hubContext;

    public SignalRNotificationService(IHubContext<TaskQueueAlertHub, ITaskQueueAlertHub> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    public async Task SendTaskQueueAlertAsync(string employeeName)
    {
        await _hubContext.Clients.All.TaskQueueAlert(employeeName);
    }
}
