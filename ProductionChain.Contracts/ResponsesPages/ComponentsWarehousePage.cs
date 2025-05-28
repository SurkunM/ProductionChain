using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.ResponsesPages;

public class ComponentsWarehousePage
{
    public List<ComponentsWarehouseItemDto> ComponentsWarehouseItems { get; set; } = new List<ComponentsWarehouseItemDto>();

    public int Total { get; set; }
}
