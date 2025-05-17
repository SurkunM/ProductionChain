using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class ProductionOrders
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }

    public required ProgressStatusType StatusType { get; set; }

    public virtual ICollection<ProductionTask> ProductionTask { get; set; } = new List<ProductionTask>();
}
