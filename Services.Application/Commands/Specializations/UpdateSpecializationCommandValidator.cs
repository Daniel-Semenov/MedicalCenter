using FluentValidation;
using Services.Application.Constants;

namespace Services.Application.Commands.Specializations;

public class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(ValidationConstants.NameMinLength)
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage($"Name must be between {ValidationConstants.NameMinLength} and {ValidationConstants.NameMaxLength} characters");

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}