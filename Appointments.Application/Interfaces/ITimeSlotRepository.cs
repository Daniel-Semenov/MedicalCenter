using Appointments.Domain.Entities;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Interfaces;

public interface ITimeSlotRepository
{
    Task<Result<TimeSlot>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TimeSlot>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlot>> GetAvailableAsync(Guid doctorId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<HashSet<DateTime>> GetExistingStartTimesAsync(Guid doctorId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TimeSlot> slots, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TimeSlot slot, CancellationToken cancellationToken = default);
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}