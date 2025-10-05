using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IServices;

namespace ProductionChain.BusinessLogic.Services;

public class TaskQueueService : ITaskQueueService
{
    private readonly Queue<TaskQueueDto> _queue;

    public TaskQueueService()
    {
        _queue = new Queue<TaskQueueDto>();
    }

    public TaskQueueDto GetNext()
    {
        return _queue.Peek();
    }

    public void EnqueueTaskQueue(TaskQueueDto taskQueueDto)
    {
        _queue.Enqueue(taskQueueDto);
    }

    public TaskQueueDto DequeueTaskQueue()
    {
        return _queue.Dequeue();
    }

    public List<TaskQueueDto> GetTaskQueueToList()
    {
        return _queue.ToList();
    }
}
