namespace ProductionChain.Contracts.Dto.Responses;

public class NotifyManagersResponse
{
    public int EmployeeId { get; set; }

    public required string FullName { get; set; }

    public required string Position { get; set; }

    public DateTime Date { get; set; }
}
