using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateAssemblyWarehouseItemHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateAssemblyWarehouseItemHandler> _logger;

    public CreateAssemblyWarehouseItemHandler(IUnitOfWork unitOfWork, ILogger<CreateAssemblyWarehouseItemHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(AssemblyWarehouseRequest assemblyWarehouseRequest)
    {
        _unitOfWork.BeginTransaction();

        var assemblyWarehouseRepository = _unitOfWork.GetRepository<IAssemblyProductionWarehouseRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();

        var product = await productsRepository.GetByIdAsync(assemblyWarehouseRequest.ProductId);

        if (product is null)
        {
            _logger.LogError("Не удалось найти продукт с id={id}.", assemblyWarehouseRequest.ProductId);

            return false;
        }

        await assemblyWarehouseRepository.CreateAsync(assemblyWarehouseRequest.ToAssemblyWarehouseModel(product));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
