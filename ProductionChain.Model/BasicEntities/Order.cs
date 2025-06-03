using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Order
{
    public int Id { get; set; }

    public required string Customer { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int ProductsCount { get; set; }

    public required ProgressStatusType StageType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AssemblyProductionOrders> ProductionOrders { get; set; } = new List<AssemblyProductionOrders>();
}
