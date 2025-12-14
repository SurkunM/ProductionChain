namespace ProductionChain.Contracts.IServices;

public interface ITokenBlacklistService
{
    bool IsTokenBlacklistedAsync(string token);

    void SetBlacklistTokenAsync(string token, int accountId);

    //Task MarkTokenAsActiveAsync(int accountId, string token);

    //Task CleanupExpiredTokensAsync();
}
