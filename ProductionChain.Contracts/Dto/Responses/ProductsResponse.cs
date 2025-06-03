namespace ProductionChain.Contracts.Dto.Responses;

public class ProductsResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }
}
