using FluentValidation;
using Profiles.Application.DTO_s.Request;
using Profiles.Shared.Common.Constants;

namespace Profiles.Application.Validators.Doctor;

public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequestDto>
{
    public CreateDoctorRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.SpecializationId)
            .NotEmpty();

        RuleFor(x => x.OfficeId)
            .NotEmpty();

        RuleFor(x => x.CareerStartYear)
            .InclusiveBetween(ValidationConstants.MinCareerStartYear, DateTime.UtcNow.Year);
    }
}