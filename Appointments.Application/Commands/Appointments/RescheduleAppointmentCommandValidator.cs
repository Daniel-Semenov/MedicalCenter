using FluentValidation;

namespace Appointments.Application.Commands.Appointments;

public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewAppointmentDate).NotEmpty();

        RuleFor(x => x)
            .Must(x => new DateTime(
                x.NewAppointmentDate.Year,
                x.NewAppointmentDate.Month,
                x.NewAppointmentDate.Day,
                x.NewStartTime.Hour,
                x.NewStartTime.Minute,
                x.NewStartTime.Second,
                DateTimeKind.Utc
            ) > DateTime.UtcNow)
            .WithMessage("New appointment date and time must be in the future.");
    }
}