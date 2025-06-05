using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IAssemblyProductionWarehouseRepository : IRepository<AssemblyProductionWarehouse>
{
    Task<AssemblyWarehousePage> GetAssemblyWarehouseItemsAsync(GetQueryParameters queryParameters);

    bool AddWarehouseItems(int productId, int productsCount);

    bool SubtractWarehouseItems(int productId, int productsCount);
}
