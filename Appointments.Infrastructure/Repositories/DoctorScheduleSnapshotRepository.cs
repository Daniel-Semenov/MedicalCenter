using Microsoft.EntityFrameworkCore;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Infrastructure.Db;

namespace Appointments.Infrastructure.Repositories;

public class DoctorScheduleSnapshotRepository : IDoctorScheduleSnapshotRepository
{
    private readonly AppointmentsDbContext _context;

    public DoctorScheduleSnapshotRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DoctorScheduleSnapshot>> GetByDoctorIdAsync(
        Guid doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.DoctorScheduleSnapshots
            .AsNoTracking()
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceForDoctorAsync(
        Guid doctorId, IEnumerable<DoctorScheduleSnapshot> entries, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _context.DoctorScheduleSnapshots
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync(cancellationToken);

        _context.DoctorScheduleSnapshots.RemoveRange(existing);
        await _context.DoctorScheduleSnapshots.AddRangeAsync(entries, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}