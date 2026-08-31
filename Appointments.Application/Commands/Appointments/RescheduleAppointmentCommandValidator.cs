using FluentValidation;

namespace Appointments.Application.Commands.Appointments;

public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewTimeSlotId).NotEmpty();
    }
}