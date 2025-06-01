namespace ProductionChain.Contracts.Dto.Responses;

public class ProductionOrderResponse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public int InProgressCount { get; set; }

    public int CompletedCount { get; set; }

    public int TotalCount { get; set; }

    public required string Status { get; set; }
}
