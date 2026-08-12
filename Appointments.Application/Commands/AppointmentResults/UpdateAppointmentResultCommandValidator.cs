using Appointments.Shared.Common.Constants;
using FluentValidation;

namespace Appointments.Application.Commands.AppointmentResults;

public class UpdateAppointmentResultCommandValidator : AbstractValidator<UpdateAppointmentResultCommand>
{
    public UpdateAppointmentResultCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

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