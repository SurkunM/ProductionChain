namespace ProductionChain.Contracts.IServices;

public interface INotificationService
{
    Task SendManagersTaskQueueNotificationAsync(string employeeName);
}
