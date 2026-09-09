using Documents.Application.Queries;
using FluentValidation;

namespace Documents.Application.Validators;

public class DownloadDocumentQueryValidator 
    : AbstractValidator<DownloadDocumentQuery>
{
    public DownloadDocumentQueryValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEqual(Guid.Empty)
            .WithMessage("Document ID is required.");
    }
}