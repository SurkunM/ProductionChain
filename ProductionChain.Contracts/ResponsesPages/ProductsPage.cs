using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductsPage
{
    public List<ProductsResponse> Products { get; set; } = new List<ProductsResponse>();

    public int Total { get; set; }
}
