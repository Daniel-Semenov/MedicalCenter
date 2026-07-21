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

    public async Task BlacklistAsync(string jti, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        await _cache.SetStringAsync(KeyPrefix + jti, "revoked",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            },
            cancellationToken);
    }

    public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(KeyPrefix + jti, cancellationToken);

        return value is not null;
    }
}