using Authorization.Application.Constants;
using Authorization.Application.DTO_s;
using FluentValidation;

namespace Authorization.Application.Validators;

public sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(ValidationConstants.RefreshTokenLength);
    }
}