namespace ProductionChain.Model.WorkflowEntities;

public class ProductionHistory
{
    public int Id { get; set; }

    public int? ProductionTaskId { get; set; }

    public virtual ProductionTask? ProductionTask { get; set; }
}
