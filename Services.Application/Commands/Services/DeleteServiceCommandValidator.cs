using FluentValidation;

namespace Services.Application.Commands.Services;

public class DeleteServiceCommandValidator : AbstractValidator<DeleteServiceCommand>
{
    public DeleteServiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}