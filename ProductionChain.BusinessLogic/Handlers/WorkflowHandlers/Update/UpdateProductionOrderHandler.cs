using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateProductionOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateProductionOrderHandler> _logger;

    public UpdateProductionOrderHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductionOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ProductionOrdersRequest productionOrdersRequest)
    {
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();

        var productionOrder = await productionOrdersRepository.GetByIdAsync(productionOrdersRequest.Id);

        if (productionOrder is null)
        {
            return false;
        }

        if (productionOrdersRequest.AddCount > 0)
        {
            productionOrder.TotalCount += productionOrdersRequest.AddCount;
        }
        else if (productionOrdersRequest.SubtractCount > 0)
        {
            productionOrder.TotalCount -= productionOrdersRequest.SubtractCount;
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
