namespace ProductionChain.Contracts.Dto.Responses;

public class ProductionHistoryResponse
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public required string Employee { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public int Count { get; set; }
}
