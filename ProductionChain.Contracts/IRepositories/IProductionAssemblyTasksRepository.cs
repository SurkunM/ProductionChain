using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductionAssemblyTasksRepository
{
    Task<ProductionTasksPage> GetTasksAsync(GetQueryParameters queryParameters);
}
