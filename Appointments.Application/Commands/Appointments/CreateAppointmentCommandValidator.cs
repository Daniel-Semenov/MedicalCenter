using FluentValidation;

namespace Appointments.Application.Commands.Appointments;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.OfficeId).NotEmpty();
        RuleFor(x => x.AppointmentDate).NotEmpty();

        RuleFor(x => x)
            .Must(x => new DateTimeOffset(
                x.AppointmentDate.Year,
                x.AppointmentDate.Month,
                x.AppointmentDate.Day,
                x.StartTime.Hour,
                x.StartTime.Minute,
                x.StartTime.Second,
                TimeSpan.Zero
            ) > DateTimeOffset.UtcNow)
            .WithMessage("Appointment date and time must be in the future.");
    }
}