using FluentValidation;
using Profiles.Application.DTO_s.Request;

namespace Profiles.Application.Validators.Receptionist;

public class CreateReceptionistRequestValidator : AbstractValidator<CreateReceptionistRequestDto>
{
    public CreateReceptionistRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.OfficeId)
            .NotEmpty();
    }
}