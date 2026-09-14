using Documents.Domain.Entities;

namespace Documents.Application.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<IReadOnlyList<Document>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
    Task<Document?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}