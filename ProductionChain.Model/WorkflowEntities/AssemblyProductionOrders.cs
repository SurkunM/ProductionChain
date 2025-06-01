using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class AssemblyProductionOrders
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public virtual required Order Order { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int InProgressCount { get; set; }

    public int CompletedCount { get; set; }

    public int TotalCount { get; set; }

    public required ProgressStatusType StatusType { get; set; }

    public virtual ICollection<AssemblyProductionTask> AssemblyTask { get; set; } = new List<AssemblyProductionTask>();
}
