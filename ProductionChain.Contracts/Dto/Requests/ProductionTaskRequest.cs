namespace ProductionChain.Contracts.Dto.Requests;

public class ProductionTaskRequest
{
    public int Id { get; set; }

    public int ProductionOrderId { get; set; }

    public int EmployeeId { get; set; }

    public int ProductId { get; set; }

    public int ProductsCount { get; set; }

    public int StatusId { get; set; }

    public DateTime StartTime { get; set; }
}
