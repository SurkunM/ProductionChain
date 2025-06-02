using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IAssemblyProductionOrdersRepository : IRepository<AssemblyProductionOrders>
{
    Task<ProductionOrdersPage> GetProductionOrdersAsync(GetQueryParameters queryParameters);

    bool AddCompletedCount(int productionOrderId, int completedCount);

    bool AddInProgressCount(int productionOrderId, int inProgressCount);

    bool SubtractInProgressCount(int productionOrderId, int inProgressCount);
}
