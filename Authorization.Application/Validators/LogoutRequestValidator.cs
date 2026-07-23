using Authorization.Application.Constants;
using Authorization.Application.DTO_s;
using FluentValidation;

namespace Authorization.Application.Validators;

public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(ValidationConstants.RefreshTokenLength);
    }
}