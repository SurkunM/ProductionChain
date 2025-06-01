using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class AssemblyProductionTask
{
    public int Id { get; set; }

    public int ProductionOrderId { get; set; }

    public virtual required AssemblyProductionOrders ProductionOrder { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }

    public int EmployeeId { get; set; }

    public virtual required Employee Employee { get; set; }

    public required ProgressStatusType ProgressStatus { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<ProductionHistory> AssemblyHistories { get; set; } = new List<ProductionHistory>();
}
