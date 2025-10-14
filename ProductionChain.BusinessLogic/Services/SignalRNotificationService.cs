using Microsoft.AspNetCore.SignalR;
using ProductionChain.BusinessLogic.Hubs;
using ProductionChain.Contracts.IHubs;
using ProductionChain.Contracts.IServices;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<TaskQueueNotificationHub, ITaskQueueNotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<TaskQueueNotificationHub, ITaskQueueNotificationHub> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    public async Task SendManagersTaskQueueNotificationAsync(string employeeName)
    {
        await _hubContext.Clients.Group(RolesEnum.Manager.ToString()).NotifyManagers(employeeName);
    }
}
