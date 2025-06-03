namespace ProductionChain.Contracts.Dto.Requests;

public class OrderRequest
{
    public int Id { get; set; }

    public required string Customer { get; set; }

    public int ProductId { get; set; }

    public int ProductsCount { get; set; }
}
