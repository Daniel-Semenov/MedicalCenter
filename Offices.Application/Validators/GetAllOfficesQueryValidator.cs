using FluentValidation;
using Offices.Application.Queries;
using Offices.Domain.Constants;

namespace Offices.Application.Validators;

public class GetAllOfficesQueryValidator : AbstractValidator<GetAllOfficesQuery>
{
    public GetAllOfficesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(ValidationConstants.MaxPageSize).WithMessage($"PageSize must not exceed {ValidationConstants.MaxPageSize}");
    }
}