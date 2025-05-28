using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class OrdersPage
{
    public List<OrderDto> Orders { get; set; } = new List<OrderDto>();

    public int Total { get; set; }
}
