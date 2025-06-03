namespace ProductionChain.Contracts.Dto.Responses;

public class ComponentsWarehouseResponse
{
    public int Id { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public required string ComponentType { get; set; }

    public int ProductsCount { get; set; }
}
