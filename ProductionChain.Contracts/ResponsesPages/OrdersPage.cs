using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class OrdersPage
{
    public List<OrderResponse> Orders { get; set; } = new List<OrderResponse>();

    public int Total { get; set; }
}
