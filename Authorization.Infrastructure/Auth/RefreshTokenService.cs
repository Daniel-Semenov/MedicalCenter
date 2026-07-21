using Microsoft.Extensions.Caching.Distributed;

namespace Authorization.Infrastructure.Auth;

public class RefreshTokenService
{
    private readonly IDistributedCache _cache;

    public RefreshTokenService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SaveAsync(Guid userId, string refreshToken, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(refreshToken, userId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            },
            cancellationToken);
    }

    public async Task<Guid?> GetUserIdAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(refreshToken, cancellationToken);

        if (value is null)
        {
            return null;
        }

        return Guid.Parse(value);
    }

    public async Task RemoveAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(refreshToken, cancellationToken);
    }
}