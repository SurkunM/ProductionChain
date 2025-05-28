namespace ProductionChain.Contracts.Dto.Requests;

public class ProductionTaskRequest
{
    public int ProductionOrderId { get; set; }

    public int EmployeeId { get; set; }

    public int ProductId { get; set; }

    public int Count { get; set; }

    public DateTime? StartTime { get; set; }
}
