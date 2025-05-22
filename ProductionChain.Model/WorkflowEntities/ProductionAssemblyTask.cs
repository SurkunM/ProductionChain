using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class ProductionAssemblyTask
{
    public int Id { get; set; }

    public int AssemblyOrdersId { get; set; }

    public virtual required ProductionAssemblyOrders AssemblyOrders { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }

    public int EmployeeId { get; set; }

    public virtual required Employee Employee { get; set; }

    public required ProgressStatusType ProgressStatus { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<ProductionAssemblyHistory> AssemblyHistories { get; set; } = new List<ProductionAssemblyHistory>();
}
