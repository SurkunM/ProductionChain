using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IComponentsWarehouseRepository : IRepository<ComponentsWarehouse>
{
    Task<ComponentsWarehousePage> GetComponentsAsync(GetQueryParameters queryParameters);

    bool TakeComponentsByProductId(int productId, int componentsCount);
}
