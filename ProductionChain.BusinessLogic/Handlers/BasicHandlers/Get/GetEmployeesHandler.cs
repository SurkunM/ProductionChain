using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;

public class GetEmployeesHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployeesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<EmployeesPage> HandleAsync(GetQueryParameters queryParameters)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        return employeeRepository.GetEmployeesAsync(queryParameters);
    }
}
