using Appointments.Shared.Common.Constants;
using FluentValidation;

namespace Appointments.Application.Commands.AppointmentResults;

public class CreateAppointmentResultCommandValidator : AbstractValidator<CreateAppointmentResultCommand>
{
    public CreateAppointmentResultCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();

        RuleFor(x => x.Complaints)
            .NotEmpty()
            .MaximumLength(AppointmentConstants.MaxComplaintsLength);

        RuleFor(x => x.Conclusion)
            .NotEmpty()
            .MaximumLength(AppointmentConstants.MaxConclusionLength);

        RuleFor(x => x.Recommendations)
            .NotEmpty()
            .MaximumLength(AppointmentConstants.MaxRecommendationsLength);
    }
}