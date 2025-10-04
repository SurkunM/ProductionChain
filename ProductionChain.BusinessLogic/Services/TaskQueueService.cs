using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IServices;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Services;

public class TaskQueueService : ITaskQueueService
{
    private readonly Queue<TaskQueueDto> _queue;

    public TaskQueueService()
    {
        _queue = new Queue<TaskQueueDto>();
    }

    public TaskQueueDto GetNextEmployee()
    {
        return _queue.Peek();
    }

    public void EnqueueEmployee(TaskQueueDto taskQueueDto)
    {
        _queue.Enqueue(taskQueueDto);
    }

    public TaskQueueDto DequeueEmployee()
    {
        return _queue.Dequeue();
    }

    public List<TaskQueueDto> GetTaskQueueToList()
    {
        return _queue.ToList();
    }
}
