using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyTasksRepository : IRepository<ProductionAssemblyTask>
{
    Task<ProductionTasksPage> GetTasksAsync(GetQueryParameters queryParameters);
}
