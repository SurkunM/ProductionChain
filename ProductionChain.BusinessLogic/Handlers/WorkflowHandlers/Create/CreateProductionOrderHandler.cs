using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateProductionOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductionOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> HandleAsync(ProductionOrdersRequest productionOrderDto)
    {
        _unitOfWork.BeginTransaction();

        var productionOrdersRepository = _unitOfWork.GetRepository<IProductionAssemblyOrdersRepository>();
        var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();

        var order = await ordersRepository.GetByIdAsync(productionOrderDto.OrderId);

        if (order is null)
        {
            return false;
        }

        var product = ordersRepository.GetOrderProduct(order);

        await productionOrdersRepository.CreateAsync(productionOrderDto.ToProductionOrderModel(order, product));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
