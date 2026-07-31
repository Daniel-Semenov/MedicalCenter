using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Interfaces.Repositories;

public interface IReceptionistRepository
{
    Task<Result<Receptionist>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Receptionist>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Receptionist>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Result<Receptionist>> CreateAsync(Receptionist receptionist, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Receptionist receptionist, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Receptionist>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}