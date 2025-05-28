using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyOrdersRepository : IRepository<ProductionAssemblyOrders>
{
    Task<ProductionOrdersPage> GetProductionOrdersAsync(GetQueryParameters queryParameters);
}
