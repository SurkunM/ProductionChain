namespace ProductionChain.Contracts.Dto;

public class HistoryDto
{
    public int Id { get; set; }

    public required string ProductName { get; set; }

    public required string EmployeeName { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
