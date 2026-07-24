using FluentValidation;
using Offices.Application.Queries;

namespace Offices.Application.Validators;

public class GetOfficeByIdQueryValidator : AbstractValidator<GetOfficeByIdQuery>
{
    public GetOfficeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}