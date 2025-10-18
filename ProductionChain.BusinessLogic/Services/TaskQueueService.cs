using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IServices;
using ProductionChain.Model.BasicEntities;

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

    public NotifyManagersResponse GenerateResponse(Employee employee)
    {
        return new NotifyManagersResponse
        {
            EmployeeId = employee.Id,
            FullName = $"{employee.LastName} {employee.FirstName[0]}.{employee.MiddleName?[0]}",
            Date = DateTime.Now,
            QueueCount = _queue.Count
        };
    }
}
