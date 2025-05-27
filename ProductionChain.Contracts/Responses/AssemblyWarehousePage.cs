using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class AssemblyWarehousePage
{
    public List<AssemblyWarehouseDto> AssemblyWarehouseItems { get; set; } = new List<AssemblyWarehouseDto>();

    public int Total { get; set; }
}
