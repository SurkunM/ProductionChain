using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class AddToTaskQueueHandler
{
    private readonly ITaskQueueService _taskQueueService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly INotificationService _notificationService;

    public AddToTaskQueueHandler(ITaskQueueService taskQueueService, IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _taskQueueService = taskQueueService ?? throw new ArgumentNullException(nameof(taskQueueService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public async Task HandleAsync(int employeeId)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var employee = await employeeRepository.GetByIdAsync(employeeId);

        if (employee is null)
        {
            throw new NotFoundException("Сотрудник не найден");
        }

        _taskQueueService.EnqueueTaskQueue(employee.ToTaskQueueDto());

        var response = _taskQueueService.GenerateResponse(employee);

        await _notificationService.SendManagersTaskQueueNotificationAsync(response);
    }
}