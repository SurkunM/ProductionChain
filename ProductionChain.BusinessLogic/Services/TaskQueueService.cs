using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IServices;
using System.Collections.Concurrent;

namespace ProductionChain.BusinessLogic.Services;

public class TaskQueueService : ITaskQueueService
{
    private readonly ConcurrentDictionary<int, TaskQueueDto> _queueDictionary;

    private readonly object _lockObj = new object();

    public TaskQueueService()
    {
        _queueDictionary = new ConcurrentDictionary<int, TaskQueueDto>();
    }

    public void AddEmployee(TaskQueueDto taskQueueDto)
    {
        if (!_queueDictionary.TryAdd(taskQueueDto.EmployeeId, taskQueueDto))
        {
            throw new InvalidOperationException("Сотрудник уже в очереди");
        }
    }

    public bool RemoveEmployee(int id)
    {
        if (_queueDictionary.IsEmpty)
        {
            throw new InvalidOperationException("Очередь пуста");
        }

        return _queueDictionary.TryRemove(id, out _);
    }


    public List<TaskQueueDto> GetTaskQueueToList()
    {
        return _queueDictionary.Values.ToList();
    }

    public bool ContainsEmployee(int employeeId)
    {
        return _queueDictionary.ContainsKey(employeeId);
    }

    public TaskQueueDto? GetEmployeeFromQueue(int employeeId)
    {
        return _queueDictionary.TryGetValue(employeeId, out var employee) ? employee : null;
    }
}
