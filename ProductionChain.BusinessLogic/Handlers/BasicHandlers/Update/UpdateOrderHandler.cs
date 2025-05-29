using Microsoft.Extensions.Logging;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.BasicHandlers.Update;

public class UpdateOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteOrderHandler> _logger;

    public UpdateOrderHandler(IUnitOfWork unitOfWork, ILogger<DeleteOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(OrderRequest orderRequest)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();
            var productRepository = _unitOfWork.GetRepository<IProductsRepository>();

            var product = await productRepository.GetByIdAsync(orderRequest.ProductId);

            if (product is null)
            {
                return false;
            }

            ordersRepository.Update(orderRequest.ToOrderModel(product));

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при редактирования заказа");

            _unitOfWork.RollbackTransaction();

            return false;
        }
    }
}
