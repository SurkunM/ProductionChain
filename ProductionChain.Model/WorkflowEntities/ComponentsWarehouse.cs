using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Model.WorkflowEntities;

public class ComponentsWarehouse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public required ComponentType Type { get; set; }

    public int Count { get; set; }
}
