using FluentValidation;
using Profiles.Application.DTO_s.Request;
using Profiles.Shared.Common.Constants;

namespace Profiles.Application.Validators.Receptionist;

public class CreateReceptionistRequestValidator : AbstractValidator<CreateReceptionistRequestDto>
{
    public CreateReceptionistRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.OfficeId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.MiddleName)
            .MaximumLength(FieldLengthConstants.NameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.EmailMaxLength)
            .EmailAddress();

        RuleFor(x => x.OfficeName)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.OfficeNameMaxLength);

        RuleFor(x => x.OfficeAddress)
            .NotEmpty()
            .MaximumLength(FieldLengthConstants.OfficeAddressMaxLength);
    }
}