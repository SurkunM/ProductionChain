namespace ProductionChain.Contracts.Dto.Requests;

public class AssemblyWarehouseRequest
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ProductsCount { get; set; }
}
