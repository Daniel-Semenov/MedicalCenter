using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.Appointments;

public class GetAppointmentByIdQuery : IRequest<Result<AppointmentDetailsDto>>
{
    public Guid Id { get; set; }
}