using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Notification;

public class NotifyEmployeeHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly INotificationService _notificationService;

    public NotifyEmployeeHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public async Task HandleAsync(int employeeId, int taskId)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var task = await tasksRepository.GetByIdAsync(taskId) ?? throw new NotFoundException("Задача не найдена");

        var response = _notificationService.GenerateNotifyEmployeeResponse(task.ToTaskQueueDto());

        await _notificationService.SendEmployeesTaskQueueNotificationAsync(employeeId, response);
    }

    public async Task HandleAsync(int employeeId)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();
        var employee = await employeeRepository.GetByIdAsync(employeeId) ?? throw new NotFoundException("Задача не найдена");

        var response = _notificationService.GenerateNotifyEmployeeResponse();

        await _notificationService.SendEmployeesTaskQueueNotificationAsync(employee.AccountId, response);
    }
}
