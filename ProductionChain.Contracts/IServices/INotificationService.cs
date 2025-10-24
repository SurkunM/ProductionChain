using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Dto.Shared;

namespace ProductionChain.Contracts.IServices;

public interface INotificationService
{
    Task SendManagersTaskQueueNotificationAsync(NotifyManagersResponse notifyManagersResponse);

    Task SendEmployeesTaskQueueNotificationAsync(int employeeId, NotifyEmployeeResponse notifyEmployeeResponse);

    NotifyManagersResponse GenerateNotifyManagersResponse(TaskQueueDto dto);

    NotifyEmployeeResponse GenerateNotifyEmployeeResponse(TaskQueueDto dto);
}