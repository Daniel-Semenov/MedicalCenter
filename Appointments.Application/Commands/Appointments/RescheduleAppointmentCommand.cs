using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class RescheduleAppointmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public Guid NewTimeSlotId { get; set; }
}