using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IServices;

namespace ProductionChain.BusinessLogic.Services;

public class TaskQueueService : ITaskQueueService
{
    private readonly Queue<TaskQueueDto> _queue;

    private readonly object _lockObj = new object();

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
        lock (_lockObj)
        {
            _queue.Enqueue(taskQueueDto);
        }
    }

    public TaskQueueDto DequeueTaskQueue()
    {
        lock (_lockObj)
        {
            if (_queue.Count <= 0)
            {
                throw new Exception();
            }

            return _queue.Dequeue();
        }
    }

    public List<TaskQueueDto> GetTaskQueueToList()
    {
        lock (_lockObj)
        {
            return _queue.ToList();
        }
    }

    public int GetCount()
    {
        lock (_lockObj)
        {
            return _queue.Count;
        }
    }
}
