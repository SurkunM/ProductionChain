namespace ProductionChain.Contracts.Dto.Shared;

public class TaskQueueDto
{
    public int EmployeeId { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    public required string Position { get; set; }

    public required DateTime CreateDate { get; set; }
}
