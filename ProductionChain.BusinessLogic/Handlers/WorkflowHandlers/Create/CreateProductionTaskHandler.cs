using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

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

        try
        {
            _unitOfWork.BeginTransaction();

            var productionOrder = await productionOrdersRepository.GetByIdAsync(taskRequest.ProductionOrderId);
            var product = await productsRepository.GetByIdAsync(taskRequest.ProductId);
            var employee = await employeesRepository.GetByIdAsync(taskRequest.EmployeeId);

            if (productionOrder is null || product is null || employee is null)
            {
                return false;
            }

            await tasksRepository.CreateAsync(taskRequest.ToTaskModel(productionOrder, product, employee,
                Model.Enums.ProgressStatusType.Pending, DateTime.UtcNow));

            await _unitOfWork.SaveAsync();
            // не изменились "In Progress Count" "TotalCount" 
            var isSubtracted = productionOrdersRepository.SubtractProductsCount(taskRequest.ProductionOrderId, taskRequest.Count);
            productionOrder.InProgressCount += taskRequest.Count;//TODO: Переделать через метод репозитория в один метод

            if (!isSubtracted)
            {
                _logger.LogError("Не удалось уменьшить количество продукции на {Count} в производственном заказе по id={ProductionOrderId}.",
                    taskRequest.Count, taskRequest.ProductionOrderId);

                return false;
            }


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
