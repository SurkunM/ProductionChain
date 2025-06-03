namespace ProductionChain.Contracts.Dto.Requests;

public class ProductionOrdersRequest
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int ProductsCount { get; set; }

    public int AddProductsCount { get; set; }

    public int SubtractProductsCount { get; set; }
}
