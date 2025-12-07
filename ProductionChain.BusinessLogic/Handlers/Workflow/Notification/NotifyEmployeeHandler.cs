using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Notification;

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
        var employeeRepository =_unitOfWork.GetRepository<IEmployeesRepository>();

        var task = await tasksRepository.GetByIdAsync(taskId) ?? throw new NotificationException("Ошибка при попытке оповестить сотрудника. Задача не найдена");
        var employee = await employeeRepository.GetByIdAsync(employeeId) ?? throw new NotificationException("Ошибка при попытке оповестить сотрудника. Сотрудник не найден");

        var response = _notificationService.GenerateNotifyEmployeeResponse(task.ToTaskQueueDto());

        await _notificationService.SendEmployeesTaskQueueNotificationAsync(employee.AccountId, response);
    }

    public async Task HandleAsync(int employeeId)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();
        var employee = await employeeRepository.GetByIdAsync(employeeId) ?? throw new NotificationException("Ошибка при попытке оповестить сотрудника. Задача не найдена");

        var response = _notificationService.GenerateNotifyEmployeeResponse();

        await _notificationService.SendEmployeesTaskQueueNotificationAsync(employee.Account?.Id, response);
    }
}
