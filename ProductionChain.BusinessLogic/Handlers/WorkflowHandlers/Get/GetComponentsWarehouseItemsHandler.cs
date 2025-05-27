using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;

public class GetComponentsWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetComponentsWarehouseItemsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ComponentsWarehousePage> HandleAsync(GetQueryParameters queryParameters)
    {
        var componentsWarehouseRepository = _unitOfWork.GetRepository<IComponentsWarehouseRepository>();

        return componentsWarehouseRepository.GetComponentsAsync(queryParameters);
    }
}
