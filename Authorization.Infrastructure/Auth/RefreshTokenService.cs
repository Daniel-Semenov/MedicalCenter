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

    public async Task SaveAsync(Guid userId, string refreshToken, TimeSpan expiration)
    {
        await _cache.SetStringAsync(refreshToken, userId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
    }

    public async Task<Guid?> GetUserIdAsync(
        string refreshToken)
    {
        var value = await _cache.GetStringAsync(refreshToken);

        if (value is null)
        {
            return null;
        }

        return Guid.Parse(value);
    }

    public async Task RemoveAsync(
        string refreshToken)
    {
        await _cache.RemoveAsync(refreshToken);
    }
}