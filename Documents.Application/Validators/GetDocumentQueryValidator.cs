using Documents.Application.Queries;
using FluentValidation;

namespace Documents.Application.Validators;

public class GetDocumentQueryValidator 
    : AbstractValidator<GetDocumentQuery>
{
    public GetDocumentQueryValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEqual(Guid.Empty)
            .WithMessage("Document ID is required.");
    }
}