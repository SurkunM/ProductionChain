namespace ProductionChain.Contracts.Settings;

public class AdminSettings
{
    public string UserName { get; set; } = string.Empty;

    public string DefaultPassword { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? MiddleName { get; set; } = string.Empty;
}
