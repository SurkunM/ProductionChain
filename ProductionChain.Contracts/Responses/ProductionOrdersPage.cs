using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class ProductionOrdersPage
{
    public List<ProductionOrdersDto> ProductionOrders { get; set; } = new List<ProductionOrdersDto>();

    public int Total { get; set; }
}
