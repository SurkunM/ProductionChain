using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;

public class GetProductionTasksHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductionTasksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ProductionTasksPage> HandleAsync(GetQueryParameters queryParameters)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();

        return tasksRepository.GetTasksAsync(queryParameters);
    }
}
