using FluentValidation;
using Offices.Application.Commands;

namespace Offices.Application.Validators;

public class DeleteOfficeCommandValidator : AbstractValidator<DeleteOfficeCommand>
{
    public DeleteOfficeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}