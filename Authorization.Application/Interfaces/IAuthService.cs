using System.Security.Claims;
using Authorization.Application.DTO_s;
using Authorization.Shared.Common;

namespace Authorization.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest dto, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, string jti, LogoutRequest dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshAsync(RefreshRequest dto, CancellationToken cancellationToken = default);
}