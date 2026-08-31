using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class CreateAppointmentCommand : IRequest<Result<Guid>>
{
    public Guid PatientId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid TimeSlotId { get; set; }
}