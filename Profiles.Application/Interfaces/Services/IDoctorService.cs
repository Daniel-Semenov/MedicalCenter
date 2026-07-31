using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Interfaces.Services;

public interface IDoctorService
{
    Task<Result<DoctorResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<DoctorResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<DoctorResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<DoctorResponseDto>>> GetBySpecializationAsync(Guid specializationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<DoctorResponseDto>> CreateAsync(CreateDoctorRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<DoctorResponseDto>> UpdateAsync(Guid id, UpdateDoctorRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}