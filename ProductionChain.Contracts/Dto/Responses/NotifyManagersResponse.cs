namespace ProductionChain.Contracts.Dto.Responses;

public class NotifyManagersResponse
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public required string FullName { get; set; }

    public DateTime Date { get; set; }

    public int QueueCount { get; set; }
}
