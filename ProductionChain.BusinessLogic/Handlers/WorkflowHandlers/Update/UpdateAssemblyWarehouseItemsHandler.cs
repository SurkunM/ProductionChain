using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateAssemblyWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateAssemblyWarehouseItemsHandler> _logger;

    public UpdateAssemblyWarehouseItemsHandler(IUnitOfWork unitOfWork, ILogger<UpdateAssemblyWarehouseItemsHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(AssemblyWarehouseRequest assemblyWarehouseDto)
    {
        var assemblyWarehouseRepository = _unitOfWork.GetRepository<IAssemblyProductionWarehouseRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var product = await productsRepository.GetByIdAsync(assemblyWarehouseDto.ProductId);

            if (product is null)
            {
                throw new NotFoundException($"Не найден продукт по ProductId: {assemblyWarehouseDto.ProductId}");
            }
            //TODO: Нужен метод в репозиторий, который будет обновлять каждый items отдельно
            assemblyWarehouseRepository.Update(assemblyWarehouseDto.ToAssemblyWarehouseModel(product)); 

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не найден продукт по id={ProductId}", assemblyWarehouseDto.ProductId);

            _unitOfWork.RollbackTransaction();

            throw;
        }
    }
}
