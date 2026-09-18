using Authorization.Application.DTO_s;
using Authorization.Domain.Entities;

namespace Authorization.Application.Interfaces;

public interface IAccountResponseFactory
{
    Task<AccountResponse> CreateAsync(Account account, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountResponse>> CreateManyAsync(
        IReadOnlyList<Account> accounts,
        CancellationToken cancellationToken = default);
}