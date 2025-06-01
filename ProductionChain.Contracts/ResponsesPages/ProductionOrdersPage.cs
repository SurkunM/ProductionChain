using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductionOrdersPage
{
    public List<ProductionOrderResponse> ProductionOrders { get; set; } = new List<ProductionOrderResponse>();

    public int Total { get; set; }
}
