using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Dto.Shared;

namespace ProductionChain.Contracts.IServices;

public interface INotificationService
{
    Task SendManagersTaskQueueNotificationAsync(int? accountId, NotifyManagersResponse notifyManagersResponse);

    Task SendEmployeesTaskQueueNotificationAsync(int accountId, NotifyEmployeeResponse notifyEmployeeResponse);

    NotifyManagersResponse GenerateNotifyManagersResponse(TaskQueueDto dto);

    NotifyEmployeeResponse GenerateNotifyEmployeeResponse(TaskQueueDto dto);

    NotifyEmployeeResponse GenerateNotifyEmployeeResponse();
}