using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IAssemblyProductionOrdersRepository : IRepository<AssemblyProductionOrders>
{
    Task<ProductionOrdersPage> GetProductionOrdersAsync(GetQueryParameters queryParameters);

    bool AddProductsCount(int orderId, int productsCount);

    bool SubtractProductsCount(int orderId, int productsCount);
}
