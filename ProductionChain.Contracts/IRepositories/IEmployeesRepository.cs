using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IEmployeesRepository
{
    Task<EmployeesPage> GetEmployeesAsync(GetQueryParameters queryParameters);
}
