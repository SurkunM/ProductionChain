using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductsPage
{
    public List<ProductsDto> Products { get; set; } = new List<ProductsDto>();

    public int Total { get; set; }
}
