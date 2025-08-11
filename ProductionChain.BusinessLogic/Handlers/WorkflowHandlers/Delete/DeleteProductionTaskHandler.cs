using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
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

    public async Task HandleAsync(ProductionTaskRequest taskRequest)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();
        var historiesRepository = _unitOfWork.GetRepository<IProductionHistoryRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var success = productionOrdersRepository.SubtractInProgressCount(taskRequest.ProductionOrderId, taskRequest.ProductsCount)
                && productionOrdersRepository.AddCompletedCount(taskRequest.ProductionOrderId, taskRequest.ProductsCount);

            if (!success)
            {
                _logger.LogError("При изменении значений \"InProgress\" и \"Completed\" в производственном заказе произошла ошибка.");

                throw new UpdateStateException("При обновлений в производственном заказе произошла ошибка");//Сделать более информативнее!
            }

            success = employeesRepository.UpdateEmployeeStatus(taskRequest.EmployeeId, EmployeeStatusType.Available)
                    && productionOrdersRepository.UpdateProductionOrderStatus(taskRequest.ProductionOrderId);

            if (!success)
            {
                _logger.LogError("При изменении статуса сотрудника и производственного заказа произошла ошибка.");

                throw new UpdateStateException("При обновлений в статуса произошла ошибка");
            }

            var task = await tasksRepository.GetByIdAsync(taskRequest.Id);

            if (task is null)
            {
                _logger.LogError("Не удалось найди задачу по переданному id={id}", taskRequest.Id);

                throw new NotFoundException("Не удалось найди задачу");
            }

            tasksRepository.SetTaskEndTime(taskRequest.Id);
            await historiesRepository.CreateAsync(task.ToHistoryModel());

            tasksRepository.Delete(task);

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении задачи из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
