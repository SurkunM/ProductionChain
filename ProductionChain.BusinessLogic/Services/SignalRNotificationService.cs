using Microsoft.AspNetCore.SignalR;
using ProductionChain.BusinessLogic.Hubs;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Dto.Shared;
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

    public async Task SendManagersTaskQueueNotificationAsync(int? accountId, NotifyManagersResponse notifyManagersResponse)
    {
        await _hubContext.Clients.Group($"{RolesEnum.Manager}_{accountId}").NotifyManagers(notifyManagersResponse);
    }

    public async Task SendEmployeesTaskQueueNotificationAsync(int? accountId, NotifyEmployeeResponse notifyEmployeeResponse)
    {
        await _hubContext.Clients.Group($"{RolesEnum.Employee}_{accountId}").NotifyEmployees(notifyEmployeeResponse);
    }

    public NotifyManagersResponse GenerateNotifyManagersResponse(TaskQueueDto dto)
    {
        return new NotifyManagersResponse
        {
            EmployeeId = dto.EmployeeId,
            FullName = dto.EmployeeFullName,
            Position = dto.Position,
            Date = dto.CreateDate
        };
    }

    public NotifyEmployeeResponse GenerateNotifyEmployeeResponse(TaskQueueDto dto)
    {
        return new NotifyEmployeeResponse
        {
            ProductName = dto.TaskProductName,
            Count = dto.ProductCount,
            HasNewTaskIssued = true
        };
    }

    public NotifyEmployeeResponse GenerateNotifyEmployeeResponse()
    {
        return new NotifyEmployeeResponse
        {
            HasNewTaskIssued = false
        };
    }
}
