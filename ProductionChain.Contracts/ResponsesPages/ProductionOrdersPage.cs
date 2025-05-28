using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductionOrdersPage
{
    public List<ProductionOrderDto> ProductionOrders { get; set; } = new List<ProductionOrderDto>();

    public int Total { get; set; }
}
