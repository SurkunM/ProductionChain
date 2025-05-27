namespace ProductionChain.Contracts.Dto;

public class ProductionOrdersDto
{
    public int Id { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public int Count { get; set; }

    public required string Status { get; set; }
}
