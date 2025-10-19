namespace ProductionChain.Contracts.Dto.Requests;

public class AuthenticationLoginRequest
{
    public required string UserLogin { get; set; }

    public required string Password { get; set; }
}
