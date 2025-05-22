using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class ProductionAssemblyOrders
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public virtual required Order Order { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }

    public required ProgressStatusType StatusType { get; set; }

    public virtual ICollection<ProductionAssemblyTask> AssemblyTask { get; set; } = new List<ProductionAssemblyTask>();
}
