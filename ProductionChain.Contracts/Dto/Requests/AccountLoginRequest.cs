namespace ProductionChain.Contracts.Dto.Requests;

public class AccountLoginRequest
{
    public required string UserLogin { get; set; }

    public required string Password { get; set; }
}
