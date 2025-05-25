using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class ProductsPage
{
    public List<ProductsDto> Products { get; set; } = new List<ProductsDto>();

    public int Total { get; set; }
}
