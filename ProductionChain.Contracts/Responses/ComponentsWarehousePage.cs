using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class ComponentsWarehousePage
{
    public List<ComponentsWarehouseItemsDto> ComponentsWarehouseItems { get; set; } = new List<ComponentsWarehouseItemsDto>();

    public int Total { get; set; }
}
