using Profiles.Domain.Entities;
using Profiles.Shared.Common;

namespace Profiles.Application.Interfaces;

public interface IPatientRepository
{
    Task<Result<Patient>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Patient>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Patient>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Result<Patient>> CreateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
}