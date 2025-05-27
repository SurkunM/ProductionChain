using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyWarehouseRepository
{
    Task<AssemblyWarehousePage> GetAssemblyWarehouseItemsAsync(GetQueryParameters queryParameters);
}
