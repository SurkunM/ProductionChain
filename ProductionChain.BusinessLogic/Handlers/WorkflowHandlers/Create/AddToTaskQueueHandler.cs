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

    private readonly IHubContext<TaskQueueAlertHub, ITaskQueueAlertHub> _hubContext;

    public AddToTaskQueueHandler(ITaskQueueService tasksQueueService, IUnitOfWork unitOfWork, IHubContext<TaskQueueAlertHub, ITaskQueueAlertHub> hubContext)
    {
        _tasksQueueService = tasksQueueService ?? throw new ArgumentNullException(nameof(tasksQueueService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
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

        if (_hubContext.Clients != null && !string.IsNullOrEmpty(employee.FirstName))
        {
            try
            {
                await _hubContext.Clients.All.TaskQueueAlert(employee.FirstName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }
    }
}
