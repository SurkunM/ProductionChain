namespace ProductionChain.Contracts.Dto.Responses;

public class ProductionHistoryResponse
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public required string Employee { get; set; }

    public required string Product { get; set; }

    public int ProductsCount { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}
