using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Get;

public class GetProductionOrdersHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductionOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ProductionOrdersPage> HandleAsync(GetQueryParameters queryParameters)
    {
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();

        return productionOrdersRepository.GetProductionOrdersAsync(queryParameters);
    }
}
