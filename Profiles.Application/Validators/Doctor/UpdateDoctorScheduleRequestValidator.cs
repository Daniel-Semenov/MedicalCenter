using FluentValidation;
using Profiles.Application.DTO_s.Request;

namespace Profiles.Application.Validators.Doctor;

public class UpdateDoctorScheduleRequestValidator : AbstractValidator<UpdateDoctorScheduleRequestDto>
{
    public UpdateDoctorScheduleRequestValidator()
    {
        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.DayOfWeek).IsInEnum();

            entry.RuleFor(e => e)
                .Must(e => e.EndTime > e.StartTime)
                .WithMessage("EndTime must be after StartTime");
        });
    }
}