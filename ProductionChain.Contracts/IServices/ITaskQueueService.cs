using ProductionChain.Contracts.Dto.Shared;

namespace ProductionChain.Contracts.IServices;

public interface ITaskQueueService
{
    void EnqueueTaskQueue(TaskQueueDto taskQueueDto);

    TaskQueueDto DequeueTaskQueue();

    List<TaskQueueDto> GetTaskQueueToList();

    TaskQueueDto GetNext();
}
