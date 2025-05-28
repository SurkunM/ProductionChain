using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateAssemblyWarehouseItemHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAssemblyWarehouseItemHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> HandleAsync(AssemblyWarehouseRequest assemblyWarehouseRequest)
    {
        _unitOfWork.BeginTransaction();

        var assemblyWarehouseRepository = _unitOfWork.GetRepository<IProductionAssemblyWarehouseRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();

        var product = await productsRepository.GetByIdAsync(assemblyWarehouseRequest.ProductId);

        if (product is null)
        {
            return false;
        }

        await assemblyWarehouseRepository.CreateAsync(assemblyWarehouseRequest.ToAssemblyWarehouseModel(product));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
