using Documents.Application.Commands;
using FluentValidation;

namespace Documents.Application.Validators;

public class DeleteDocumentCommandValidator 
    : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEqual(Guid.Empty)
            .WithMessage("Document ID is required.");
    }
}