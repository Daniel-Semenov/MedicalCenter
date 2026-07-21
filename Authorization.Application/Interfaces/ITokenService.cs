using Authorization.Application.DTO_s;
using Authorization.Domain.Entities;
using Authorization.Shared.Common;

namespace Authorization.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GenerateTokensAsync(Account account, CancellationToken cancellationToken = default);
    Task<Guid?> GetRefreshTokenOwnerAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAccessTokenAsync(string jti, CancellationToken cancellationToken = default);
    Task<TokenResult> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken = default);
}