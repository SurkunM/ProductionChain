using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Update;

public class UpdateComponentsWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateComponentsWarehouseItemsHandler> _logger;

    public UpdateComponentsWarehouseItemsHandler(IUnitOfWork unitOfWork, ILogger<UpdateComponentsWarehouseItemsHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ComponentsWarehouseRequest componentsWarehouseDto)
    {
        var componentsWarehouseRepository = _unitOfWork.GetRepository<IComponentsWarehouseRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var product = await productsRepository.GetByIdAsync(componentsWarehouseDto.ProductId);

            if (product is null)
            {
                throw new NotFoundException($"Не найден продукт по ProductId: {componentsWarehouseDto.ProductId}");
            }

            componentsWarehouseRepository.Update(componentsWarehouseDto.ToComponentsWarehouseModel(product));

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не найден продукт по id={ProductId}", componentsWarehouseDto.ProductId);

            _unitOfWork.RollbackTransaction();

            throw;
        }
    }
}
