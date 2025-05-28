using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IEmployeesRepository : IRepository<Employee>
{
    Task<EmployeesPage> GetEmployeesAsync(GetQueryParameters queryParameters);
}
