namespace ProductionChain.Contracts.Dto.Requests;

public class ProductionOrdersRequest
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int OrderStatus { get; set; }

    public int Count { get; set; }
}
