namespace ProductionChain.Contracts.IHubs;

public interface ITaskQueueNotificationHub
{
    Task NotifyManagers(string employee);
}
