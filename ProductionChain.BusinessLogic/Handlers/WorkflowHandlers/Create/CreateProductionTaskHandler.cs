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

                _unitOfWork.RollbackTransaction();

                return false;
            }

            var success = componentsWarehouseRepository.TakeComponentsByProductId(taskRequest.ProductId, taskRequest.ProductsCount);

            if (!success)
            {
                _logger.LogError("Не найдено нужно количество компонентов count={Count} для продукта id={ProductId}", taskRequest.ProductsCount, taskRequest.ProductId);

                _unitOfWork.RollbackTransaction();

                return false;
            }

            success = productionOrdersRepository.AddInProgressCount(taskRequest.ProductionOrderId, taskRequest.ProductsCount);

            if (!success)
            {
                _logger.LogError("Не удалось увеличить значение строки InProgress на {Count} в производственном заказе.", taskRequest.ProductsCount);

                _unitOfWork.RollbackTransaction();

                return false;
            }

            success = employeesRepository.UpdateEmployeeStatus(taskRequest.EmployeeId, EmployeeStatusType.Busy)
                && productionOrdersRepository.UpdateProductionOrderStatus(taskRequest.ProductionOrderId);

            if (!success)
            {
                _logger.LogError("При изменении статуса сотрудника и производственного заказа произошла ошибка.");

                _unitOfWork.RollbackTransaction();

                return false;
            }

            await tasksRepository.CreateAsync(taskRequest.ToTaskModel(productionOrder, product, employee, DateTime.UtcNow));

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка. Транзакция отменена.");

            return false;
        }
    }
}
