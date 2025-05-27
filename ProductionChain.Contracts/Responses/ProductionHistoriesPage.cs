using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class ProductionHistoriesPage
{
    public List<HistoriesDto> Histories { get; set; } = new List<HistoriesDto>();

    public int Total { get; set; }
}
