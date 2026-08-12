using Appointments.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Infrastructure.Db;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;

namespace Appointments.Infrastructure.Repositories;

public class SnapshotRepository<TSnapshot> : ISnapshotRepository<TSnapshot>
    where TSnapshot : class, ISnapshotEntity
{
    private readonly AppointmentsDbContext _context;

    public SnapshotRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TSnapshot>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await _context.Set<TSnapshot>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (snapshot is null)
        {
            return Result<TSnapshot>.Failure(SnapshotErrors.NotFound(typeof(TSnapshot).Name, id));
        }

        return Result<TSnapshot>.Success(snapshot);
    }

    public async Task UpsertAsync(TSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.Set<TSnapshot>()
            .AnyAsync(s => s.Id == snapshot.Id, cancellationToken);

        if (exists)
        {
            _context.Set<TSnapshot>().Update(snapshot);
        }
        else
        {
            await _context.Set<TSnapshot>().AddAsync(snapshot, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}