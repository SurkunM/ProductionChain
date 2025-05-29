using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateComponentsWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateComponentsWarehouseItemsHandler> _logger;

    public UpdateComponentsWarehouseItemsHandler(IUnitOfWork unitOfWork, ILogger<UpdateComponentsWarehouseItemsHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ComponentsWarehouseRequest componentsWarehouseRequest)
    {
        var componentsWarehouseRepository = _unitOfWork.GetRepository<IProductionAssemblyWarehouseRepository>();

        var componentsWarehouseItem = await componentsWarehouseRepository.GetByIdAsync(componentsWarehouseRequest.Id);

        if (componentsWarehouseItem is null)//TODO: в request приходит componentType. найти по нему продукт или не получать componentType
        {
            return false;
        }

        if (componentsWarehouseRequest.AddCount > 0)
        {
            componentsWarehouseItem.Count += componentsWarehouseRequest.AddCount;
        }
        else if (componentsWarehouseRequest.SubtractCount > 0)
        {
            componentsWarehouseItem.Count -= componentsWarehouseRequest.SubtractCount;
        }
        else
        {
            _logger.LogError("Переданы пустые значения AddCount/SubtractCount. Изменения в складе компонентов не выполнены.");

            return false;
        }

        await _unitOfWork.SaveAsync();

        return true;
    }
}
