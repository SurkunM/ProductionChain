namespace ProductionChain.Model.BasicEntities;

public class Warehouse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }
}
