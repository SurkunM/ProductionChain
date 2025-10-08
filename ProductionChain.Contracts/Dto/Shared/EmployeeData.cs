using ProductionChain.Model.Enums;

namespace ProductionChain.Contracts.Dto.Shared;

public class EmployeeData
{
    public int UserId { get; set; }

    public required string UserName { get; set; }

    public required IList<string> Roles { get; set; }
}
