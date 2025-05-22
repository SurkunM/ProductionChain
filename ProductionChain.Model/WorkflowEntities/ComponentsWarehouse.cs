using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Model.WorkflowEntities;

public class ComponentsWarehouse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public required string Name { get; set; }

    public int Count { get; set; }
}
