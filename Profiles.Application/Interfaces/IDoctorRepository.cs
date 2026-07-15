using Profiles.Domain.Entities;
using Profiles.Shared.Common;

namespace Profiles.Application.Interfaces;

public interface IDoctorRepository
{
    Task<Result<Doctor>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Doctor>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Doctor>>> GetBySpecializationAsync(Guid specializationId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Doctor>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    
    Task<Result<Doctor>> CreateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default);
}