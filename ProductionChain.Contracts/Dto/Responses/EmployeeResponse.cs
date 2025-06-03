namespace ProductionChain.Contracts.Dto.Responses;

public class EmployeeResponse
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    public required string Position { get; set; }

    public required string Status { get; set; }
}
