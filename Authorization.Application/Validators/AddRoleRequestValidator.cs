using Authorization.Application.Constants;
using Authorization.Application.DTO_s;
using Authorization.Domain.Constants;
using FluentValidation;

namespace Authorization.Application.Validators;

public sealed class AddRoleRequestValidator : AbstractValidator<AddRoleRequest>
{
    public AddRoleRequestValidator()
    {
        RuleFor(x => x.Role)
            .Must(role => Roles.All.Contains(role))
            .WithMessage("Unknown role.");
    }
}