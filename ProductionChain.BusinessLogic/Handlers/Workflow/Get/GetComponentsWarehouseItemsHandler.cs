using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Get;

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
