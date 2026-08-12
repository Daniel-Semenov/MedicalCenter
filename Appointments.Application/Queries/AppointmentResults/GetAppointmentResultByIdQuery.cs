using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.AppointmentResults;

public class GetAppointmentResultByIdQuery : IRequest<Result<AppointmentResultDto>>
{
    public Guid Id { get; set; }
}