namespace ProductionChain.Contracts.Dto;

public class ComponentsWarehouseItemDto
{
    public int Id { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public required string ComponentType { get; set; }

    public int Count { get; set; }
}
