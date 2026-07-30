using FluentValidation;
using Services.Application.Constants;

namespace Services.Application.Commands.Specializations;

public class CreateSpecializationCommandValidator : AbstractValidator<CreateSpecializationCommand>
{
    public CreateSpecializationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(ValidationConstants.NameMinLength)
            .MaximumLength(ValidationConstants.NameMaxLength)
            .WithMessage($"Name must be between {ValidationConstants.NameMinLength} and {ValidationConstants.NameMaxLength} characters");

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Services)
            .NotEmpty()
            .WithMessage("At least one service is required");

        RuleForEach(x => x.Services)
            .ChildRules(service =>
            {
                service.RuleFor(s => s.Name)
                    .NotEmpty()
                    .MinimumLength(ValidationConstants.NameMinLength)
                    .MaximumLength(ValidationConstants.NameMaxLength);

                service.RuleFor(s => s.Price)
                    .GreaterThan(ValidationConstants.PriceMinValue)
                    .WithMessage($"Price must be greater than {ValidationConstants.PriceMinValue}");

                service.RuleFor(s => s.Category)
                    .IsInEnum();

                service.RuleFor(s => s.Status)
                    .IsInEnum();
            });
    }
}