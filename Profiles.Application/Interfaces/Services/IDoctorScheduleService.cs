using Profiles.Application.DTO_s.Request;
using Profiles.Application.DTO_s.Response;
using Profiles.Shared.Common;

namespace Profiles.Application.Interfaces.Services;

public interface IDoctorScheduleService
{
    Task<Result<DoctorScheduleResponseDto>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<Result<DoctorScheduleResponseDto>> UpdateAsync(Guid doctorId, UpdateDoctorScheduleRequestDto request, CancellationToken cancellationToken = default);
}