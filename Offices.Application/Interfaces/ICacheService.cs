namespace Offices.Application.Interfaces;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan ttl,
        CancellationToken cancellationToken = default) where T : class;

    Task<long> GetVersionAsync(string scope, CancellationToken cancellationToken = default);

    Task BumpVersionAsync(string scope, CancellationToken cancellationToken = default);
}