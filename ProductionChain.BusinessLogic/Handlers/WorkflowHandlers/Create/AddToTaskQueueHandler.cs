using Microsoft.AspNetCore.SignalR;
using ProductionChain.BusinessLogic.Hubs;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IHubs;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class AddToTaskQueueHandler
{
    private readonly ITaskQueueService _tasksQueueService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly INotificationService _notificationService;

    public AddToTaskQueueHandler(ITaskQueueService tasksQueueService, IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _tasksQueueService = tasksQueueService ?? throw new ArgumentNullException(nameof(tasksQueueService));
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

        _tasksQueueService.EnqueueTaskQueue(employee.ToTaskQueueDto());

        await _notificationService.SendManagersTaskQueueNotificationAsync(employee.FirstName);
    }
}