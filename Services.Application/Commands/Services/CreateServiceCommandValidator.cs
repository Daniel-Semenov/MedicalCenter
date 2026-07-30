using FluentValidation;
using Services.Application.Constants;

namespace Services.Application.Commands.Services;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(ValidationConstants.NameMinLength)
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage($"Name must be between {ValidationConstants.NameMinLength} and {ValidationConstants.NameMaxLength} characters");

        RuleFor(x => x.Price)
            .GreaterThan(ValidationConstants.PriceMinValue)
            .WithMessage($"Price must be greater than {ValidationConstants.PriceMinValue}");

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.SpecializationId)
            .NotEmpty();
    }
}