using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class ProductionTask
{
    public int Id { get; set; }

    public int ProductionOrdersId { get; set; }

    public virtual required ProductionOrders ProductionOrders { get; set; }

    public int EmployeeId { get; set; }

    public virtual required Employee Employee { get; set; }

    public required ProductionStageType ProductionStage { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<ProductionHistory> Histories { get; set; } = new List<ProductionHistory>();
}
