using FluentValidation;
using Services.Application.Constants;
using Services.Application.Interfaces;

namespace Services.Application.Queries;

public class PagedQueryValidator : AbstractValidator<IPagedQuery>
{
    public PagedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(ValidationConstants.DefaultPageNumber)
            .WithMessage($"Page must be greater than or equal to {ValidationConstants.DefaultPageNumber}");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(ValidationConstants.DefaultPageSize)
            .LessThanOrEqualTo(ValidationConstants.MaxPageSize)
            .WithMessage($"PageSize must be between {ValidationConstants.DefaultPageSize} and {ValidationConstants.MaxPageSize}");
    }
}
