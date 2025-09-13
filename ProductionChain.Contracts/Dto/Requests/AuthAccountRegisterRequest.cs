namespace ProductionChain.Contracts.Dto.Requests;

public class AuthAccountRegisterRequest
{
    public int EmployeeId { get; set; }

    public string Login { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
