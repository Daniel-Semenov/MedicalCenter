using Authorization.Application.DTO_s;
using Authorization.Shared.Common;
using MediatR;

namespace Authorization.Application.Queries.Accounts;

public class GetAccountByIdQuery : IRequest<Result<AccountResponse>>
{
    public Guid Id { get; set; }
}