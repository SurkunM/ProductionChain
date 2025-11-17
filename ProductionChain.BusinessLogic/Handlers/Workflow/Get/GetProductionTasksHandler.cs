using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Get;

public class GetProductionTasksHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly string _employeeRole = RolesEnum.Employee.ToString();

    public GetProductionTasksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ProductionTasksPage> HandleAsync(GetQueryParameters queryParameters, AccountContext accountContext)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();

        if (accountContext.Role == _employeeRole)
        {
            return tasksRepository.GetEmployeeTasksAsync(queryParameters, accountContext.EmployeeId);
        }

        return tasksRepository.GetTasksAsync(queryParameters);
    }
}
