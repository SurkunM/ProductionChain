namespace ProductionChain.Model.WorkflowEntities;

public class ProductionHistory
{
    public int Id { get; set; }

    public int ProductionTaskId { get; set; }

    public virtual required AssemblyProductionTask ProductionTask { get; set; }

    public required string Employee { get; set; }

    public required string  Product { get; set; }

    public int ProductsCount { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}
