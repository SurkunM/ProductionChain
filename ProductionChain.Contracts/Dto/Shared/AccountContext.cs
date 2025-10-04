namespace ProductionChain.Contracts.Dto.Shared;

public class AccountContext
{
    public int AccountId { get; set; }

    public required string Role { get; set; }

    public int EmployeeId { get; set; }
}
