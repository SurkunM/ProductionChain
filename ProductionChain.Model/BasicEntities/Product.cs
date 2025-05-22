using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductionAssemblyOrders> AssemblyOrders { get; set; } = new List<ProductionAssemblyOrders>();

    public virtual ICollection<ProductionAssemblyTask> AssemblyTask { get; set; } = new List<ProductionAssemblyTask>();

    public virtual ICollection<ProductionAssemblyWarehouse> AssemblyWarehouse { get; set; } = new List<ProductionAssemblyWarehouse>();

    public virtual ICollection<ComponentsWarehouse> ComponentsWarehouse { get; set; } = new List<ComponentsWarehouse>();
}
