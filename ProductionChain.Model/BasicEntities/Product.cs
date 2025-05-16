using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Model.BasicEntities;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }

    public virtual ICollection<ProductionOrders> ProductionOrders { get; set; } = new List<ProductionOrders>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
