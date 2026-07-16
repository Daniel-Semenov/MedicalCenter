using Authorization.Application.DTO_s;
using Authorization.Domain.Entities;

namespace Authorization.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GenerateTokensAsync(
        Account account,
        CancellationToken cancellationToken = default);
}