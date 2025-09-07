using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Employee
{
    public int Id { get; set; }

    public required string LastName { get; set; }

    public required string FirstName { get; set; }

    public string? MiddleName { get; set; }

    public int? AccountId { get; set; }

    public Account? Account { get; set; }

    public int? ChiefId { get; set; }

    public virtual Employee? Chief { get; set; }

    public required EmployeePositionType Position { get; set; }

    public required EmployeeStatusType Status { get; set; }

    public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

    public virtual ICollection<AssemblyProductionTask> ProductionTasks { get; set; } = new List<AssemblyProductionTask>();
}
