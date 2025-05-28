namespace ProductionChain.Contracts.Dto;

public class AssemblyWarehouseItemDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Model { get; set; }

    public int Count { get; set; }
}
