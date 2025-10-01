using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.IServices;

public interface ITaskQueueService
{
    void EnqueueEmployee(Employee employee);

    Employee DequeueEmployee();

    List<Employee> GetTaskQueueToList();

    Employee GetNextEmployee();
}
