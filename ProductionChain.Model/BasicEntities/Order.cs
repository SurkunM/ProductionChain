using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Order
{
    public int Id { get; set; }

    public required string Customer { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }

    public required string StageType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ProductionAssemblyOrders> AssemblyOrders { get; set; } = new List<ProductionAssemblyOrders>();
}
