using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IComponentsWarehouseRepository
{
    Task<ComponentsWarehousePage> GetComponentsAsync(GetQueryParameters queryParameters);
}
