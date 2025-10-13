namespace ProductionChain.Contracts.IServices;

public interface INotificationService
{
    Task SendTaskQueueAlertAsync(string employeeName);
}
