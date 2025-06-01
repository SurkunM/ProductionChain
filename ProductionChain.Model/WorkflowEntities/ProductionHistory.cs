namespace ProductionChain.Model.WorkflowEntities;

public class ProductionHistory
{
    public int Id { get; set; }

    public int? AssemblyTaskId { get; set; }

    public virtual AssemblyProductionTask? AssemblyTask { get; set; }
}
