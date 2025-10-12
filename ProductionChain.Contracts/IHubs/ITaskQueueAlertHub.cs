namespace ProductionChain.Contracts.IHubs;

public interface ITaskQueueAlertHub
{
    Task TaskQueueAlert(string employee);
}
