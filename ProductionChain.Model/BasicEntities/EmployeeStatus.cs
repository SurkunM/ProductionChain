using ProductionChain.Model.Enums;

namespace ProductionChain.Model.BasicEntities;

public class EmployeeStatus
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public virtual required Employee Employee { get; set; }

    public required string StatusType { get; set; }
}
