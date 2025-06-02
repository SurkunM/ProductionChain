using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class DeleteProductionTaskHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteProductionTaskHandler> _logger;

    public DeleteProductionTaskHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductionTaskHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ProductionTaskRequest taskRequest)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var success = productionOrdersRepository.SubtractInProgressCount(taskRequest.ProductionOrderId, taskRequest.Count)
                && productionOrdersRepository.AddCompletedCount(taskRequest.ProductionOrderId, taskRequest.Count);

            if (!success)
            {
                _logger.LogError("При изменении счётчиков \"InProgress\" и \"Completed\" произошла ошибка.");

                return false;
            }

            success = employeesRepository.UpdateStatusByEmployeeId(taskRequest.EmployeeId, EmployeeStatusType.Available);

            if (!success)
            {
                _logger.LogError("При изменении статуса сотрудника произошла ошибка.");

                return false;
            }

            var task = await tasksRepository.GetByIdAsync(taskRequest.Id);

            if (task is null)
            {
                return false;
            }

            tasksRepository.Delete(task);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении задачи из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
