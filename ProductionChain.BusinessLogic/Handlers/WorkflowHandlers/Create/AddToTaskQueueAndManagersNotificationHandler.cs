using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class AddToTaskQueueAndManagersNotificationHandler
{
    private readonly ITaskQueueService _taskQueueService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly INotificationService _notificationService;

    public AddToTaskQueueAndManagersNotificationHandler(ITaskQueueService taskQueueService, IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _taskQueueService = taskQueueService ?? throw new ArgumentNullException(nameof(taskQueueService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public async Task HandleAsync(int employeeId)
    {
        if (_taskQueueService.ContainsEmployee(employeeId))
        {
            throw new InvalidOperationException("Сотрудник уже добавлен");
        }

        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var employee = await employeeRepository.GetByIdAsync(employeeId) ?? throw new NotFoundException("Сотрудник не найден");
        var employeeDto = employee.ToTaskQueueDto();

        _taskQueueService.AddEmployee(employeeDto);

        var response = _notificationService.GenerateNotifyManagersResponse(employeeDto);

        if (employee.ChiefId is null)
        {
            throw new NotFoundException("У сотрудника не найден руководитель");
        }

        await _notificationService.SendManagersTaskQueueNotificationAsync(employee.ChiefId, response);
    }
}