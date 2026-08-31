using Appointments.Domain.Entities.Snapshots;

namespace Appointments.Application.Interfaces;

public interface IDoctorSnapshotQuery
{
    Task<IReadOnlyList<DoctorSnapshot>> GetActiveAsync(CancellationToken cancellationToken = default);
}