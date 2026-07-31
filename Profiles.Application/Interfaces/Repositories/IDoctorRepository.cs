using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Interfaces.Repositories;

public interface IDoctorRepository
{
    Task<Result<Doctor>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Doctor>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Doctor>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Result<Doctor>> CreateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Doctor>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Doctor>>> GetBySpecializationPagedAsync(Guid specializationId, int page, int pageSize, CancellationToken cancellationToken = default);
}
