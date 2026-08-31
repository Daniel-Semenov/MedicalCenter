using Profiles.Domain.Entities;
using Profiles.Shared.Common;

namespace Profiles.Application.Interfaces.Repositories;

public interface IDoctorScheduleRepository
{
    Task<Result<IReadOnlyList<DoctorSchedule>>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<Result> ReplaceForDoctorAsync(Guid doctorId, IEnumerable<DoctorSchedule> entries, CancellationToken cancellationToken = default);
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}