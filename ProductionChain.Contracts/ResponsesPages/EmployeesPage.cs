using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class EmployeesPage
{
    public List<EmployeeResponse> Employees { get; set; } = new List<EmployeeResponse>();

    public int TotalCount { get; set; }
}
