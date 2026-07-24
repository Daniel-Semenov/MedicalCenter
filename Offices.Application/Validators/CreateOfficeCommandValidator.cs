using FluentValidation;
using Offices.Application.Commands;
using Offices.Domain.Constants;

namespace Offices.Application.Validators;

public class CreateOfficeCommandValidator : AbstractValidator<CreateOfficeCommand>
{
    public CreateOfficeCommandValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(ValidationConstants.MaxCityLength).WithMessage($"City must not exceed {ValidationConstants.MaxCityLength} characters");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(ValidationConstants.MaxStreetLength).WithMessage($"Street must not exceed {ValidationConstants.MaxStreetLength} characters");

        RuleFor(x => x.HouseNumber)
            .GreaterThan(0).WithMessage("House number must be positive");

        RuleFor(x => x.OfficeNumber)
            .GreaterThan(0).WithMessage("Office number must be positive");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(ValidationConstants.PhoneRegexPattern).WithMessage("Invalid phone number format. Use international format (e.g., +1234567890)");
    }
}