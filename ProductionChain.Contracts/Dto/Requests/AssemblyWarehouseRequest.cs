namespace ProductionChain.Contracts.Dto.Requests;

public class AssemblyWarehouseRequest
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Count { get; set; }

    public int AddCount { get; set; }

    public int SubtractCount { get; set; }
}
