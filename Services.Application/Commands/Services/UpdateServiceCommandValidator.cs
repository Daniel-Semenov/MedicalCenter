using FluentValidation;
using Services.Application.Constants;

namespace Services.Application.Commands.Services;

public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(ValidationConstants.NameMinLength)
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage($"Name must be between {ValidationConstants.NameMinLength} and {ValidationConstants.NameMaxLength} characters");

        RuleFor(x => x.Price)
            .GreaterThan(ValidationConstants.PriceMinValue)
            .WithMessage($"Price must be greater than {ValidationConstants.PriceMinValue}");

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}