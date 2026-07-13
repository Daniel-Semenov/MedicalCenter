using Profiles.DAL.Entities;

namespace Profiles.DAL.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Patient> CreateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}