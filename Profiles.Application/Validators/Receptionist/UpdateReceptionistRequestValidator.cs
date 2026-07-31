using FluentValidation;
using Profiles.Application.DTO_s.Request;

namespace Profiles.Application.Validators.Receptionist;

public class UpdateReceptionistRequestValidator : AbstractValidator<UpdateReceptionistRequestDto>
{
    public UpdateReceptionistRequestValidator()
    {
        RuleFor(x => x.OfficeId)
            .NotEmpty();
    }
}