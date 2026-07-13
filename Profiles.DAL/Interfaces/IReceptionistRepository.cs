using Profiles.DAL.Entities;

namespace Profiles.DAL.Repositories;

public interface IReceptionistRepository
{
    Task<Receptionist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Receptionist?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Receptionist> CreateAsync(Receptionist receptionist, CancellationToken cancellationToken = default);
    Task UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken = default);
    Task<IEnumerable<Receptionist>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}