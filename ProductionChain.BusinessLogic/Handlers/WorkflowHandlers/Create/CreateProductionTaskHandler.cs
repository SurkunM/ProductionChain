using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateProductionTaskHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateProductionTaskHandler> _logger;

    public CreateProductionTaskHandler(IUnitOfWork unitOfWork, ILogger<CreateProductionTaskHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
    }

    public async Task<bool> HandleAsync(ProductionTaskRequest taskRequest)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();
        var componentsWarehouseRepository = _unitOfWork.GetRepository<IComponentsWarehouseRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var productionOrder = await productionOrdersRepository.GetByIdAsync(taskRequest.ProductionOrderId);
            var product = await productsRepository.GetByIdAsync(taskRequest.ProductId);
            var employee = await employeesRepository.GetByIdAsync(taskRequest.EmployeeId);

            if (productionOrder is null || product is null || employee is null)
            {
                _logger.LogError("Не удалось найти все сущности по переданным id для создания задачи: {ProductionOrderId}, {ProductId}, {EmployeeId}.",
                    taskRequest.ProductionOrderId, taskRequest.ProductId, taskRequest.EmployeeId);

                return false;
            }

            await tasksRepository.CreateAsync(taskRequest.ToTaskModel(productionOrder, product, employee,
                ProgressStatusType.Pending, DateTime.UtcNow));

            var success = componentsWarehouseRepository.TakeComponentsByProductId(taskRequest.ProductId, taskRequest.Count);

            if (!success)
            {
                _logger.LogError("Не найдено нужно количество компонентов count={Count} для продукта id={ProductId}", taskRequest.Count, taskRequest.ProductId);

                return false;
            }

            success = productionOrdersRepository.AddInProgressCount(taskRequest.ProductionOrderId, taskRequest.Count);

            if (!success)
            {
                _logger.LogError("Не удалось увеличить значение строки InProgress на {Count} .", taskRequest.Count);

                return false;
            }

            success = employeesRepository.UpdateStatusByEmployeeId(taskRequest.EmployeeId, EmployeeStatusType.Busy);

            if (!success)
            {
                _logger.LogError("Не удалось обновить статус сотрудника по id={EmployeeId} .", taskRequest.EmployeeId);

                return false;
            }

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception e)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(e, "Ошибка. Транзакция отменена.");

            return false;
        }
    }
}
