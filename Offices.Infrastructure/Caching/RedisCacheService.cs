using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Offices.Application.Interfaces;
using StackExchange.Redis;

namespace Offices.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan ttl,
        CancellationToken cancellationToken = default) where T : class
    {
        var cached = await TryGetAsync<T>(key);

        if (cached is not null)
        {
            _logger.LogDebug("Cache hit: {Key}", key);
            return cached;
        }

        var gate = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);

        try
        {
            cached = await TryGetAsync<T>(key);

            if (cached is not null)
            {
                return cached;
            }

            _logger.LogDebug("Cache miss: {Key}", key);

            var value = await factory(cancellationToken);
            await TrySetAsync(key, value, ttl);

            return value;
        }
        finally
        {
            gate.Release();
            _locks.TryRemove(key, out _);
        }
    }

    public async Task<long> GetVersionAsync(string scope, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _redis.GetDatabase().StringGetAsync(VersionKey(scope));

            return value.HasValue && value.TryParse(out long version) ? version : 0;
        }
        catch (Exception ex) when (ex is RedisException or TimeoutException)
        {
            _logger.LogWarning(ex, "Redis unavailable while reading version of {Scope}", scope);
            return 0;
        }
    }

    public async Task BumpVersionAsync(string scope, CancellationToken cancellationToken = default)
    {
        try
        {
            await _redis.GetDatabase().StringIncrementAsync(VersionKey(scope));
        }
        catch (Exception ex) when (ex is RedisException or TimeoutException)
        {
            _logger.LogWarning(ex, "Redis unavailable while bumping version of {Scope}", scope);
        }
    }

    private async Task<T?> TryGetAsync<T>(string key) where T : class
    {
        try
        {
            var raw = await _redis.GetDatabase().StringGetAsync(key);

            return raw.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<T>(raw.ToString(), JsonOptions);
        }
        catch (Exception ex) when (ex is RedisException or TimeoutException or JsonException)
        {
            _logger.LogWarning(ex, "Cache read failed for {Key}", key);
            return null;
        }
    }

    private async Task TrySetAsync<T>(string key, T value, TimeSpan ttl) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            await _redis.GetDatabase().StringSetAsync(key, json, WithJitter(ttl));
        }
        catch (Exception ex) when (ex is RedisException or TimeoutException)
        {
            _logger.LogWarning(ex, "Cache write failed for {Key}", key);
        }
    }

    private static TimeSpan WithJitter(TimeSpan ttl) =>
        ttl + TimeSpan.FromSeconds(Random.Shared.Next(0, (int)(ttl.TotalSeconds * 0.1) + 1));

    private static string VersionKey(string scope) => $"{scope}:ver";
}