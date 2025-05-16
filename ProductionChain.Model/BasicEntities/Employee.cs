using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Employee
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    public required EmployeePositionType Position { get; set; }

    public virtual ICollection<ProductionTask> Tasks { get; set; } = new List<ProductionTask>();
}
