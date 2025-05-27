using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyHistoryRepository
{
    Task<ProductionHistoriesPage> GetProductionHistoriesAsync(GetQueryParameters queryParameters);
}
