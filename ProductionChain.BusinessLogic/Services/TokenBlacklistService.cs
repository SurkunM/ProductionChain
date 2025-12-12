using ProductionChain.Contracts.IServices;

namespace ProductionChain.BusinessLogic.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    public Task BlacklistTokenAsync(string token, int accountId)
    {
        return Task.FromResult(0);
    }

    public Task CleanupExpiredTokensAsync()
    {
        return Task.FromResult(0);
    }

    public Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return Task.FromResult(false);
    }

    public Task MarkTokenAsActiveAsync(int accountId, string token)
    {
        return Task.FromResult(0);
    }
}
