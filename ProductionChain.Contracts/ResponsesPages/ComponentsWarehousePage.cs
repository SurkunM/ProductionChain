using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Contracts.ResponsesPages;

public class ComponentsWarehousePage
{
    public List<ComponentsWarehouseResponse> ComponentsWarehouseItems { get; set; } = new List<ComponentsWarehouseResponse>();

    public int Total { get; set; }
}
