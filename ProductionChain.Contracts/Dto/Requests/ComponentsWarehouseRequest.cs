using ProductionChain.Model.Enums;

namespace ProductionChain.Contracts.Dto.Requests;

public class ComponentsWarehouseRequest
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public required ComponentType Type { get; set; }

    public int ComponentsCount { get; set; }
}
