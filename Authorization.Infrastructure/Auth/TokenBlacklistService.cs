using Microsoft.Extensions.Caching.Distributed;

namespace Authorization.Infrastructure.Auth;

public class TokenBlacklistService
{
    private const string KeyPrefix = "blacklist:";

    private readonly IDistributedCache _cache;

    public TokenBlacklistService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task BlacklistAsync(string jti, TimeSpan ttl)
    {
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        await _cache.SetStringAsync(KeyPrefix + jti, "revoked",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });
    }

    public async Task<bool> IsBlacklistedAsync(string jti)
    {
        var value = await _cache.GetStringAsync(KeyPrefix + jti);

        return value is not null;
    }
}