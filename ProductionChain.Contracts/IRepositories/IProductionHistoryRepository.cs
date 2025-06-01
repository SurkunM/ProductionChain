using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionHistoryRepository : IRepository<ProductionHistory>
{
    Task<ProductionHistoriesPage> GetProductionHistoriesAsync(GetQueryParameters queryParameters);
}
