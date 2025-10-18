using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.IServices;

public interface ITaskQueueService
{
    void EnqueueTaskQueue(TaskQueueDto taskQueueDto);

    NotifyManagersResponse GenerateResponse(Employee employee);

    TaskQueueDto DequeueTaskQueue();

    List<TaskQueueDto> GetTaskQueueToList();

    TaskQueueDto GetNext();

    int GetCount();
}
