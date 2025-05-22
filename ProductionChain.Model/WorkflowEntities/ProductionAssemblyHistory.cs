namespace ProductionChain.Model.WorkflowEntities;

public class ProductionAssemblyHistory
{
    public int Id { get; set; }

    public int? AssemblyTaskId { get; set; }

    public virtual ProductionAssemblyTask? AssemblyTask { get; set; }
}
