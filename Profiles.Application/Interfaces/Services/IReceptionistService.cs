using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Interfaces.Services;

public interface IReceptionistService
{
    Task<Result<ReceptionistResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ReceptionistResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ReceptionistResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<ReceptionistResponseDto>> CreateAsync(CreateReceptionistRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<ReceptionistResponseDto>> UpdateAsync(Guid id, UpdateReceptionistRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}