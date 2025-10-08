using ProductionChain.Contracts.Dto.Shared;

namespace ProductionChain.Contracts.Dto.Responses;

public class AuthLoginResponse
{
    public required string Token { get; set; }

    public required EmployeeData UserData { get; set; }
}
