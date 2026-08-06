using FluentValidation;
using Profiles.Application.DTO_s.Request;
using Profiles.Shared.Common.Constants;

namespace Profiles.Application.Validators.Patient;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequestDto>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.MiddleName)
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(PhoneNumberConstants.Pattern);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow);
    }
}