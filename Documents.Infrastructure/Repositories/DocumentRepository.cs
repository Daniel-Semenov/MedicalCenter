using Documents.Application.Interfaces.Repositories;
using Documents.Domain.Entities;
using Documents.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.Repositories;

public sealed class DocumentRepository : IDocumentRepository
{
    private readonly DocumentsDbContext _context;

    public DocumentRepository(DocumentsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Document>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (idList.Count == 0)
        {
            return Array.Empty<Document>();
        }

        return await _context.Documents
            .Where(d => idList.Contains(d.Id) && !d.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public Task<Document?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Documents
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddAsync(
            document,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}