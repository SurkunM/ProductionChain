using ProductionChain.Model.Enums;

namespace ProductionChain.Model.BasicEntities;

public class Order
{
    public int Id { get; set; }

    public required string Customer { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int Count { get; set; }

    public required ProgressStatusType StageType { get; set; }
}
