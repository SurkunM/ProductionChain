using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductionHistoriesPage
{
    public List<ProductionHistoryResponse> Histories { get; set; } = new List<ProductionHistoryResponse>();

    public int TotalCount { get; set; }
}
