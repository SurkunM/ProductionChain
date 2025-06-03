namespace ProductionChain.Contracts.Dto.Responses;

public class AssemblyWarehouseItemResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }

    public int ProductsCount { get; set; }
}
