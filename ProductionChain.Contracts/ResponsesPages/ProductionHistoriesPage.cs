using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductionHistoriesPage
{
    public List<HistoryDto> Histories { get; set; } = new List<HistoryDto>();

    public int Total { get; set; }
}
