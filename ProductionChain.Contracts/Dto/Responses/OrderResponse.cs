namespace ProductionChain.Contracts.Dto.Responses;

public class OrderResponse
{
    public int Id { get; set; }

    public required string Customer { get; set; }

    public int ProductId { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public int ProductsCount { get; set; }

    public int AvailableCount { get; set; }

    public required string Status { get; set; }

    public DateTime? CreateAt { get; set; }
}
