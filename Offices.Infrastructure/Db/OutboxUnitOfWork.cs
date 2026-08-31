using Offices.Application.Interfaces;

namespace Offices.Infrastructure.Db;

public class OutboxUnitOfWork : IOutboxUnitOfWork
{
    private readonly OfficesOutboxDbContext _context;

    public OutboxUnitOfWork(OfficesOutboxDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}