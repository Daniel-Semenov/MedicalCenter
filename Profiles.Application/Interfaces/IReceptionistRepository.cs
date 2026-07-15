using Profiles.Domain.Entities;
using Profiles.Shared.Common;

namespace Profiles.Application.Interfaces;

public interface IReceptionistRepository
{
    Task<Result<Receptionist>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Receptionist>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Receptionist>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Result<Receptionist>> CreateAsync(Receptionist receptionist, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken = default);
}