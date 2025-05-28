using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyHistoryRepository : IRepository<ProductionAssemblyHistory>
{
    Task<ProductionHistoriesPage> GetProductionHistoriesAsync(GetQueryParameters queryParameters);
}
