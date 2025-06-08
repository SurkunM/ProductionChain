using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IAssemblyProductionTasksRepository : IRepository<AssemblyProductionTask>
{
    Task<ProductionTasksPage> GetTasksAsync(GetQueryParameters queryParameters);

    void SetTaskEndTime(int id);
}
