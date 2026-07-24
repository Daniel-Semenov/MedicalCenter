using FluentValidation;
using Offices.Application.Commands;

namespace Offices.Application.Validators;

public class ToggleOfficeStatusCommandValidator : AbstractValidator<ToggleOfficeStatusCommand>
{
    public ToggleOfficeStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}