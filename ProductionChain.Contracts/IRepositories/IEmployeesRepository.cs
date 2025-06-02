using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Contracts.IRepositories;

public interface IEmployeesRepository : IRepository<Employee>
{
    Task<EmployeesPage> GetEmployeesAsync(GetQueryParameters queryParameters);

    bool UpdateStatusByEmployeeId(int employeeId, EmployeeStatusType statusType);
}
