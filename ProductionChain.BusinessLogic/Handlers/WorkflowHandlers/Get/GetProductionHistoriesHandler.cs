using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;

public class GetProductionHistoriesHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductionHistoriesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ProductionHistoriesPage> HandleAsync(GetQueryParameters queryParameters)
    {
        var productionHistoriyRepository = _unitOfWork.GetRepository<IProductionHistoryRepository>();

        return productionHistoriyRepository.GetProductionHistoriesAsync(queryParameters);
    }
}
