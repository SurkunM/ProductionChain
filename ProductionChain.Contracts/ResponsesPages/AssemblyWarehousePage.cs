using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class AssemblyWarehousePage
{
    public List<AssemblyWarehouseItemDto> AssemblyWarehouseItems { get; set; } = new List<AssemblyWarehouseItemDto>();

    public int Total { get; set; }
}
