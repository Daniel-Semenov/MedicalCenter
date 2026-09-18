using FluentValidation;

namespace Authorization.Application.Queries.Accounts;

public class GetAccountByIdQueryValidator : AbstractValidator<GetAccountByIdQuery>
{
    public GetAccountByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}