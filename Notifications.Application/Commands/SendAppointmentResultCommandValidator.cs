using FluentValidation;
using Notification.Shared.Common.Constants;

namespace Notifications.Application.Commands;

public sealed class SendAppointmentResultCommandValidator
    : AbstractValidator<SendAppointmentResultCommand>
{

    public SendAppointmentResultCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();

        RuleFor(x => x.AppointmentResultId)
            .NotEmpty();

        RuleFor(x => x.PatientEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(ValidationConstants.EmailMaxLength);

        RuleFor(x => x.PatientFullName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.FullNameMaxLength);

        RuleFor(x => x.ResultDate)
            .NotEmpty();

        RuleFor(x => x.DoctorFullName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.FullNameMaxLength);

        RuleFor(x => x.DoctorSpecialization)
            .NotEmpty()
            .MaximumLength(ValidationConstants.SpecializationMaxLength);

        RuleFor(x => x.ServiceName)
            .NotEmpty()
            .MaximumLength(ValidationConstants.ServiceNameMaxLength);

        RuleFor(x => x.PdfDocumentId)
            .NotEmpty();
    }
}