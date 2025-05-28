using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class EmployeesPage
{
    public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();

    public int Total { get; set; }
}
