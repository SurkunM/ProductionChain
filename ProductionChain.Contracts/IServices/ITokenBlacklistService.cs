namespace ProductionChain.Contracts.IServices;

public interface ITokenBlacklistService
{
    Task<bool> IsTokenBlacklistedAsync(string token);

    Task BlacklistTokenAsync(string token, int accountId);

    Task MarkTokenAsActiveAsync(int accountId, string token);

    Task CleanupExpiredTokensAsync();
}
