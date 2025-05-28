using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.BasicHandlers.Create;

public class CreateOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> HandleAsync(OrderRequest orderRequest)
    {
        var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();
        var productRepository = _unitOfWork.GetRepository<IProductsRepository>();

        var product = await productRepository.GetByIdAsync(orderRequest.ProductId);

        if (product is null)
        {
            return false;
        }

        await ordersRepository.CreateAsync(orderRequest.ToOrderModel(product));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
