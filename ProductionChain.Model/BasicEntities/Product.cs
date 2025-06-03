using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<AssemblyProductionOrders> ProductionOrders { get; set; } = new List<AssemblyProductionOrders>();

    public virtual ICollection<AssemblyProductionTask> ProductionTask { get; set; } = new List<AssemblyProductionTask>();

    public virtual ICollection<AssemblyProductionWarehouse> AssemblyWarehouse { get; set; } = new List<AssemblyProductionWarehouse>();

    public virtual ICollection<ComponentsWarehouse> ComponentsWarehouse { get; set; } = new List<ComponentsWarehouse>();
}
