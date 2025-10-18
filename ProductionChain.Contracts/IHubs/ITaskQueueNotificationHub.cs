using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.IHubs;

public interface ITaskQueueNotificationHub
{
    Task NotifyManagers(NotifyManagersResponse notifyManagersResponse);
}
