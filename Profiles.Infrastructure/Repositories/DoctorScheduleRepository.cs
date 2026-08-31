using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Domain.Entities;
using Profiles.Infrastructure.Db;
using Profiles.Shared.Common;

namespace Profiles.Infrastructure.Repositories;

public class DoctorScheduleRepository : IDoctorScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<DoctorSchedule>>> GetByDoctorIdAsync(
        Guid doctorId, CancellationToken cancellationToken = default)
    {
        var entries = await _context.DoctorSchedules
            .AsNoTracking()
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<DoctorSchedule>>.Success(entries);
    }

    public async Task<Result> ReplaceForDoctorAsync(
        Guid doctorId, IEnumerable<DoctorSchedule> entries, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _context.DoctorSchedules
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync(cancellationToken);

        _context.DoctorSchedules.RemoveRange(existing);
        await _context.DoctorSchedules.AddRangeAsync(entries, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}