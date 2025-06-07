using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class AssemblyWarehousePage
{
    public List<AssemblyWarehouseItemResponse> AssemblyWarehouseItems { get; set; } = new List<AssemblyWarehouseItemResponse>();

    public int TotalCount { get; set; }
}
