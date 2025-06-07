using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class ProductionTasksPage
{
    public List<ProductionTaskResponse> Tasks { get; set; } = new List<ProductionTaskResponse>();

    public int TotalCount { get; set; }
}
