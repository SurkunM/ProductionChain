namespace ProductionChain.Contracts.Dto.Shared;

public class TaskQueueDto
{
    public int EmployeeId { get; set; }

    public string EmployeeFullName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public required DateTime CreateDate { get; set; }

    public string TaskProductName { get; set; } = string.Empty;

    public int ProductCount { get; set; }
}
