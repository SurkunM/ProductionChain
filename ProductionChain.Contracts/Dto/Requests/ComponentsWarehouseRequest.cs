namespace ProductionChain.Contracts.Dto.Requests;

public class ComponentsWarehouseRequest
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ComponentType { get; set; }

    public int ProductsCount { get; set; }

    public int AddProductsCount { get; set; }

    public int SubtractProductsCount { get; set; }
}
