using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class AssemblyProductionOrder
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public virtual required Order Order { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int InProgressProductsCount { get; set; }

    public int CompletedProductsCount { get; set; }

    public int TotalProductsCount { get; set; }

    public required ProgressStatusType StatusType { get; set; }

    public virtual ICollection<AssemblyProductionTask> ProductionTask { get; set; } = new List<AssemblyProductionTask>();
}
