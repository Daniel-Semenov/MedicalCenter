using Appointments.Domain.Entities.Snapshots;

namespace Appointments.Application.Interfaces;

public interface IDoctorScheduleSnapshotRepository
{
    Task<IReadOnlyList<DoctorScheduleSnapshot>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task ReplaceForDoctorAsync(Guid doctorId, IEnumerable<DoctorScheduleSnapshot> entries, CancellationToken cancellationToken = default);
}