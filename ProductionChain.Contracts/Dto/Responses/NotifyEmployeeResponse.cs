namespace ProductionChain.Contracts.Dto.Responses;

public class NotifyEmployeeResponse
{
    public string? ProductName { get; set; }

    public int Count { get; set; }

    public bool HasNewTaskIssued { get; set; }
}
