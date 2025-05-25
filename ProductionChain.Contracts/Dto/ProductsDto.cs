namespace ProductionChain.Contracts.Dto;

public class ProductsDto
{
    public int Id { get; set; }

    public int Index { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }
}
