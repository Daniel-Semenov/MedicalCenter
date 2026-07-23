using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Authorization.Shared.Common;
using Authorization.Shared.Common.Errors;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<Account> _userManager;
    private readonly ITokenService _tokenService;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AuthService(UserManager<Account> userManager, ITokenService tokenService, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _roleManager = roleManager;
    }


    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest dto,
        CancellationToken cancellationToken = default)
    {
        var existingAccount = await _userManager.FindByEmailAsync(dto.Email);

        if (existingAccount is not null)
        {
            return Result<AuthResponse>.Failure(
                AuthErrors.EmailAlreadyExists(dto.Email));
        }


        var account = new Account
        {
            UserName = dto.Email,
            Email = dto.Email,

            FirstName = dto.FirstName,
            LastName = dto.LastName,

            CreatedAt = DateTime.UtcNow
        };


        var result = await _userManager.CreateAsync(account, dto.Password);


        if (!result.Succeeded)
        {
            var message = string.Join(
                ", ",
                result.Errors.Select(x => x.Description));


            return Result<AuthResponse>.Failure(
                AuthErrors.RegistrationFailed(message));
        }


        var tokens = await _tokenService.GenerateTokensAsync(account, cancellationToken);


        return Result<AuthResponse>.Success(
            new AuthResponse(
                account.Id,
                account.Email!,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresAt));
    }


    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest dto,
        CancellationToken cancellationToken = default)
    {
        var account = await _userManager.FindByEmailAsync(dto.Email);


        if (account is null)
        {
            return Result<AuthResponse>.Failure(
                AuthErrors.InvalidCredentials);
        }


        var passwordValid = await _userManager.CheckPasswordAsync(account, dto.Password);


        if (!passwordValid)
        {
            return Result<AuthResponse>.Failure(
                AuthErrors.InvalidCredentials);
        }


        var tokens = await _tokenService.GenerateTokensAsync(account, cancellationToken);


        return Result<AuthResponse>.Success(
            new AuthResponse(
                account.Id,
                account.Email!,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresAt));
    }


    public async Task<Result> LogoutAsync(
        Guid userId,
        string jti,
        LogoutRequest dto,
        CancellationToken cancellationToken = default)
    {
        var refreshOwnerId = await _tokenService.GetRefreshTokenOwnerAsync(dto.RefreshToken, cancellationToken);

        if (refreshOwnerId is null || refreshOwnerId != userId)
        {
            return Result.Failure(
                AuthErrors.LogoutFailed("Refresh token is invalid or does not belong to the current user."));
        }

        await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken, cancellationToken);
        await _tokenService.RevokeAccessTokenAsync(jti, cancellationToken);

        return Result.Success();
    }
    
    public async Task<Result<AuthResponse>> RefreshAsync(
        RefreshRequest dto,
        CancellationToken cancellationToken = default)
    {
        var ownerId = await _tokenService.GetRefreshTokenOwnerAsync(
            dto.RefreshToken,
            cancellationToken);

        if (ownerId is null)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken(dto.RefreshToken));
        }

        var account = await _userManager.FindByIdAsync(ownerId.Value.ToString());

        if (account is null || !account.IsActive)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken(dto.RefreshToken));
        }

        var tokens = await _tokenService.RefreshTokensAsync(
            dto.RefreshToken,
            cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            account.Id,
            account.Email!,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresAt));
    }
    
    public async Task<Result> AddRoleAsync(
        Guid accountId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var account = await _userManager.FindByIdAsync(accountId.ToString());

        if (account is null)
        {
            return Result.Failure(AuthErrors.UserNotFound);
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Result.Failure(AuthErrors.RoleNotFound);
        }

        var userRoles = await _userManager.GetRolesAsync(account);

        if (userRoles.Any())
        {
            return Result.Failure(AuthErrors.UserAlreadyHasRole);
        }

        var result = await _userManager.AddToRoleAsync(account, role);

        if (!result.Succeeded)
        {
            var error = string.Join(
                ", ",
                result.Errors.Select(x => x.Description));

            return Result.Failure(AuthErrors.RoleAssignmentFailed(error));
        }

        return Result.Success();
    }
}