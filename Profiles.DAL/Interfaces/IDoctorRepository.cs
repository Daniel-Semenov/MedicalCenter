using Profiles.DAL.Entities;

namespace Profiles.DAL.Repositories;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(Guid specializationId, CancellationToken cancellationToken = default);
    Task<Doctor> CreateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}