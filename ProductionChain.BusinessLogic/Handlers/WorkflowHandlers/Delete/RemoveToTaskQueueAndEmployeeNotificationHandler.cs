using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class RemoveToTaskQueueAndEmployeeNotificationHandler
{
    private readonly ITaskQueueService _taskQueueService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly INotificationService _notificationService;

    public RemoveToTaskQueueAndEmployeeNotificationHandler(ITaskQueueService taskQueueService, IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _taskQueueService = taskQueueService ?? throw new ArgumentNullException(nameof(taskQueueService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public async Task HandleAsync(int employeeId, int taskId)
    {
        if (!_taskQueueService.ContainsEmployee(employeeId))
        {
            throw new NotFoundException("Сотрудник не найден");
        }

        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var task = await tasksRepository.GetByIdAsync(taskId) ?? throw new NotFoundException("Задача не найдена");
        var taskDto = task.ToTaskQueueDto();

        _taskQueueService.RemoveEmployee(employeeId);

        var response = _notificationService.GenerateNotifyEmployeeResponse(taskDto);

        await _notificationService.SendEmployeesTaskQueueNotificationAsync(employeeId, response);
    }
}
