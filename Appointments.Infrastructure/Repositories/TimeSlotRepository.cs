using Microsoft.EntityFrameworkCore;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Infrastructure.Db;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;

namespace Appointments.Infrastructure.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly AppointmentsDbContext _context;

    public TimeSlotRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TimeSlot>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var slot = await _context.TimeSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return slot is null
            ? Result<TimeSlot>.Failure(TimeSlotErrors.NotFound(id))
            : Result<TimeSlot>.Success(slot);
    }

    public async Task<Result<TimeSlot>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var slot = await _context.TimeSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.AppointmentId == appointmentId, cancellationToken);

        return slot is null
            ? Result<TimeSlot>.Failure(TimeSlotErrors.NotFound(appointmentId))
            : Result<TimeSlot>.Success(slot);
    }

    public async Task<IReadOnlyList<TimeSlot>> GetAvailableAsync(
        Guid doctorId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlots
            .AsNoTracking()
            .Where(t => t.DoctorId == doctorId
                        && t.Status == TimeSlotStatus.Available
                        && t.StartTime >= from
                        && t.StartTime < to)
            .OrderBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<HashSet<DateTime>> GetExistingStartTimesAsync(
        Guid doctorId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var starts = await _context.TimeSlots
            .AsNoTracking()
            .Where(t => t.DoctorId == doctorId && t.StartTime >= from && t.StartTime < to)
            .Select(t => t.StartTime)
            .ToListAsync(cancellationToken);

        return starts.ToHashSet();
    }

    public async Task AddRangeAsync(IEnumerable<TimeSlot> slots, CancellationToken cancellationToken = default)
    {
        await _context.TimeSlots.AddRangeAsync(slots, cancellationToken);
    }

    public async Task<Result> UpdateAsync(TimeSlot slot, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.TimeSlots.AnyAsync(t => t.Id == slot.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(TimeSlotErrors.NotFound(slot.Id));
        }

        _context.TimeSlots.Update(slot);

        return Result.Success();
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}