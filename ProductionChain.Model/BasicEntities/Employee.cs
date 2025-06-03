using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Employee
{
    public int Id { get; set; }

    public required string LastName { get; set; }

    public required string FirstName { get; set; }

    public string? MiddleName { get; set; }

    public required EmployeePositionType Position { get; set; }

    public required EmployeeStatusType Status { get; set; }

    public virtual ICollection<AssemblyProductionTask> ProductionTasks { get; set; } = new List<AssemblyProductionTask>();
}
