using Microsoft.Extensions.Caching.Memory;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.Settings;
using System.Security.Cryptography;
using System.Text;

namespace ProductionChain.BusinessLogic.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache _cache;

    private readonly JwtSettings _settings;

    public TokenBlacklistService(IMemoryCache cache, JwtSettings settings)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void SetBlacklistTokenAsync(string token, int accountId)
    {
        var hash = ComputeTokenHash(token);
        var expiry = TimeSpan.FromHours(_settings.ExpiryHours);

        _cache.Set(hash, accountId, expiry);
    }

    public bool IsTokenBlacklistedAsync(string token)
    {
        var hash = ComputeTokenHash(token);
        var blackListToken = _cache.Get(hash);

        return blackListToken is not null;
    }

    private static string ComputeTokenHash(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(hash);
    }
}
