using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyOrdersRepository
{
    Task<ProductionOrdersPage> GetProductionOrdersAsync(GetQueryParameters queryParameters);
}
