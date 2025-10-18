using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.IServices;

public interface INotificationService
{
    Task SendManagersTaskQueueNotificationAsync(NotifyManagersResponse notifyManagersResponse);
}
