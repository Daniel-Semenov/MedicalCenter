using FluentValidation;

namespace Services.Application.Commands.Specializations;

public class DeleteSpecializationCommandValidator : AbstractValidator<DeleteSpecializationCommand>
{
    public DeleteSpecializationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}