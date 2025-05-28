using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyWarehouseRepository : IRepository<ProductionAssemblyWarehouse>
{
    Task<AssemblyWarehousePage> GetAssemblyWarehouseItemsAsync(GetQueryParameters queryParameters);
}
