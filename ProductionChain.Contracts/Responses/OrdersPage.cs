using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class OrdersPage
{
    public List<OrdersDto> Orders { get; set; } = new List<OrdersDto>();

    public int Total { get; set; }
}
