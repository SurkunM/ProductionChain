using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Create;

public class CreateProductionOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateProductionOrderHandler> _logger;

    public CreateProductionOrderHandler(IUnitOfWork unitOfWork, ILogger<CreateProductionOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ProductionOrdersRequest productionOrdersRequest)
    {
        var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();
        var productRepository = _unitOfWork.GetRepository<IProductsRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var order = await ordersRepository.GetByIdAsync(productionOrdersRequest.OrderId);
            var product = await productRepository.GetByIdAsync(productionOrdersRequest.ProductId);

            if (order is null || product is null)
            {
                _logger.LogError("Не удалось найти сущности по переданному параметру {OrderId} или {ProductId}.",
                    productionOrdersRequest.OrderId, productionOrdersRequest.ProductId);

                throw new NotFoundException("Не удалось найти заказ или продукт по переданным параметрам");
            }

            if (!ordersRepository.IsOrderPending(productionOrdersRequest.OrderId))
            {
                _logger.LogError("Попытка начать заказ который уже находится в состоянии \"isProgress\" или \"Done\".");

                throw new InvalidStateException("Не корректное состояние заказа");
            }

            await productionOrdersRepository.CreateAsync(order.ToProductionOrderModel(product));

            ordersRepository.UpdateOrderStatus(productionOrdersRequest.OrderId, ProgressStatusType.InProgress);

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка. Транзакция отменена");

            _unitOfWork.RollbackTransaction();

            throw;
        }
    }
}
