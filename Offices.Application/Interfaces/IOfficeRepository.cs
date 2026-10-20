using Offices.Domain.Entities;

namespace Offices.Application.Interfaces;

public interface IOfficeRepository
{
    Task<Office?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Office office, CancellationToken cancellationToken = default);
    Task UpdateAsync(Office office, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Office> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default);
}