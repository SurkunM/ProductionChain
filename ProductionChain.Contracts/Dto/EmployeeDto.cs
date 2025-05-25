namespace ProductionChain.Contracts.Dto;

public class EmployeeDto
{
    public int Id { get; set; }

    public int Index { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    public required string Position { get; set; }

    public required string Status { get; set; } 
}
