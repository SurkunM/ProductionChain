using ProductionChain.Contracts.Dto.Shared;

namespace ProductionChain.Contracts.IServices;

public interface ITaskQueueService
{
    void EnqueueEmployee(TaskQueueDto taskQueueDto);

    TaskQueueDto DequeueEmployee();

    List<TaskQueueDto> GetTaskQueueToList();

    TaskQueueDto GetNextEmployee();
}
