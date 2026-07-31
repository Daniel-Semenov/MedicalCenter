using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Pagination;

namespace Profiles.Application.Interfaces.Services;

public interface IPatientService
{
    Task<Result<PatientResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PatientResponseDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PatientResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<PatientResponseDto>> CreateAsync(CreatePatientRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<PatientResponseDto>> UpdateAsync(UpdatePatientRequestDto request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}