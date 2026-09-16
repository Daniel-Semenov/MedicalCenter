using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Interfaces;

public interface IAppointmentResultDtoBuilder
{
    Task<Result<AppointmentResultDto>> BuildAsync(
        AppointmentResult result,
        CancellationToken cancellationToken = default);
}