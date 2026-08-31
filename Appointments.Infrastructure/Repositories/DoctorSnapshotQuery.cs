using Microsoft.EntityFrameworkCore;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Infrastructure.Db;

namespace Appointments.Infrastructure.Repositories;

public class DoctorSnapshotQuery : IDoctorSnapshotQuery
{
    private readonly AppointmentsDbContext _context;

    public DoctorSnapshotQuery(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DoctorSnapshot>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DoctorSnapshots
            .AsNoTracking()
            .Where(d => d.IsActive)
            .ToListAsync(cancellationToken);
    }
}