using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Authorization.Shared.Common;
using MediatR;

namespace Authorization.Application.Queries.Accounts;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<AccountResponse>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountResponseFactory _accountResponseFactory;

    public GetAccountByIdQueryHandler(
        IAccountRepository accountRepository,
        IAccountResponseFactory accountResponseFactory)
    {
        _accountRepository = accountRepository;
        _accountResponseFactory = accountResponseFactory;
    }

    public async Task<Result<AccountResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var getResult = await _accountRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result<AccountResponse>.Failure(getResult.Error!);
        }

        var response = await _accountResponseFactory.CreateAsync(getResult.Value!, cancellationToken);

        return Result<AccountResponse>.Success(response);
    }
}