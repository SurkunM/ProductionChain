using ProductionChain.Contracts.Dto.Shared;

namespace ProductionChain.Contracts.IServices;

public interface ITaskQueueService
{
    void AddEmployee(TaskQueueDto taskQueueDto);

    bool RemoveEmployee(int id);

    bool ContainsEmployee(int employeeId);

    TaskQueueDto? GetEmployeeFromQueue(int employeeId);

    List<TaskQueueDto> GetTaskQueueToList();
}