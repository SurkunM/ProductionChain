using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.DataAccess.UnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateTaskQueueHandler
{
    private readonly ITaskQueueService _tasksQueueService;

    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskQueueHandler(ITaskQueueService tasksQueueService, UnitOfWork unitOfWork)
    {
        _tasksQueueService = tasksQueueService ?? throw new ArgumentNullException(nameof(tasksQueueService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task HandleAsync(int employeeId)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var employee = await employeeRepository.GetByIdAsync(employeeId);

        if(employee is null)
        {
            throw new NotFoundException("Сотрудник не найден");
        }

        _tasksQueueService.EnqueueEmployee(employee.ToTaskQueueDto());
    } 
}
