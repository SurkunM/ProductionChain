namespace ProductionChain.Contracts.Dto.Responses;

public class ProductionTaskResponse
{
    public int Id { get; set; }

    public int ProductionOrderId { get; set; }

    public required string EmployeeFirstName { get; set; }

    public required string EmployeeLastName { get; set; }

    public string? EmployeeMiddleName { get; set; }

    public required string ProductName { get; set; }

    public required string ProductModel { get; set; }

    public int Count { get; set; }

    public required string Status { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
