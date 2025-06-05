using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateProductionOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateProductionOrderHandler> _logger;

    public CreateProductionOrderHandler(IUnitOfWork unitOfWork, ILogger<CreateProductionOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ProductionOrdersRequest productionOrdersRequest)
    {
        var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();
        var productRepository = _unitOfWork.GetRepository<IProductsRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();

        _unitOfWork.BeginTransaction();

        try
        {
            if (!ordersRepository.IsOrderPending(productionOrdersRequest.OrderId))
            {
                _logger.LogError("Ошибка. Попытка начать задачу которая уже находится в состоянии \"isProgress\" или \"Done\".");

                return false;
            }

            var order = await ordersRepository.GetByIdAsync(productionOrdersRequest.OrderId);
            var product = await productRepository.GetByIdAsync(productionOrdersRequest.ProductId);

            if (order is null || product is null)
            {
                _logger.LogError("Не удалось найти сущности по переданному параметру {OrderId} или {ProductId}.",
                    productionOrdersRequest.OrderId, productionOrdersRequest.ProductId);

                return false;
            }

            await productionOrdersRepository.CreateAsync(productionOrdersRequest.ToProductionOrderModel(order, product));

            ordersRepository.UpdateStatusByOrderId(productionOrdersRequest.OrderId, ProgressStatusType.InProgress);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка. Транзакция отменена");

            _unitOfWork.BeginTransaction();

            return false;
        }
    }
}
