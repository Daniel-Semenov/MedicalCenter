using FluentValidation;
using Profiles.Application.DTO_s.Request;

namespace Profiles.Application.Validators.Patient;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequestDto>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}