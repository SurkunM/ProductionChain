using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IServices;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Delete;

public class RemoveEmployeeFromTaskQueueHandler
{
    private readonly ITaskQueueService _taskQueueService;

    public RemoveEmployeeFromTaskQueueHandler(ITaskQueueService taskQueueService)
    {
        _taskQueueService = taskQueueService ?? throw new ArgumentNullException(nameof(taskQueueService));
    }

    public void Handle(int employeeId)
    {
        if (!_taskQueueService.ContainsEmployee(employeeId))
        {
            throw new NotFoundException("Сотрудник не найден");
        }

        _taskQueueService.RemoveEmployee(employeeId);
    }
}
