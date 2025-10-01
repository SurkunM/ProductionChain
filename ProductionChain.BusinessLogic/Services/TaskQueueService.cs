using ProductionChain.Contracts.IServices;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Services;

public class TaskQueueService : ITaskQueueService
{
    private readonly Queue<Employee> _queue;

    public TaskQueueService()
    {
        _queue = new Queue<Employee>();
    }

    public Employee GetNextEmployee()
    {
        return _queue.Peek();
    }

    public void EnqueueEmployee(Employee employee)
    {
        _queue.Enqueue(employee);
    }

    public Employee DequeueEmployee()
    {
        return _queue.Dequeue();
    }

    public List<Employee> GetTaskQueueToList()
    {
        return _queue.ToList();
    }
}
