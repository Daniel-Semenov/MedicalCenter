using Documents.Application.Queries;
using Documents.Shared.Common.Constants;
using FluentValidation;

namespace Documents.Application.Validators;

public class GetPresignedUrlsQueryValidator : AbstractValidator<GetPresignedUrlsQuery>
{
    public GetPresignedUrlsQueryValidator()
    {
        RuleFor(x => x.DocumentIds)
            .NotEmpty()
            .WithMessage("At least one document id is required.")
            .Must(ids => ids.Count <= PresignedUrlConstants.MaxBatchSize)
            .WithMessage($"No more than {PresignedUrlConstants.MaxBatchSize} document ids can be requested at once.");

        RuleForEach(x => x.DocumentIds)
            .NotEqual(Guid.Empty);
    }
}