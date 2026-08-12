using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class CancelAppointmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}