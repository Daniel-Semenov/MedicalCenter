using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Infrastructure.Auth;

public class JwtTokenService : ITokenService
{
    private readonly RefreshTokenGenerator _refreshGenerator;
    private readonly RefreshTokenService _refreshService;
    private readonly TokenBlacklistService _blacklistService;
    private readonly JwtOptions _options;
    private readonly UserManager<Account> _userManager;


    public JwtTokenService(
        IOptions<JwtOptions> options,
        RefreshTokenGenerator refreshGenerator,
        RefreshTokenService refreshService,
        TokenBlacklistService blacklistService, UserManager<Account> userManager)
    {
        _refreshGenerator = refreshGenerator;
        _refreshService = refreshService;
        _blacklistService = blacklistService;
        _userManager = userManager;
        _options = options.Value;
    }


    public async Task<TokenResult> GenerateTokensAsync(Account account, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),

            new(JwtRegisteredClaimNames.Email, account.Email!),

            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = _refreshGenerator.Generate();

        await _refreshService.SaveAsync(
            account.Id,
            refreshToken,
            TimeSpan.FromHours(_options.RefreshTokenExpirationHours), cancellationToken);

        return new TokenResult(
            accessToken,
            refreshToken,
            expiresAt);
    }


    public async Task<Guid?> GetRefreshTokenOwnerAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _refreshService.GetUserIdAsync(refreshToken, cancellationToken);
    }


    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        await _refreshService.RemoveAsync(refreshToken, cancellationToken);
    }


    public async Task RevokeAccessTokenAsync(string jti, CancellationToken cancellationToken = default)
    {
        await _blacklistService.BlacklistAsync(
            jti,
            TimeSpan.FromMinutes(_options.ExpirationMinutes));
    }
    
    public async Task<TokenResult> RefreshTokensAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var ownerId = await _refreshService.GetUserIdAsync(refreshToken, cancellationToken);

        var account = await _userManager.FindByIdAsync(ownerId.Value.ToString());

        await _refreshService.RemoveAsync(refreshToken, cancellationToken);

        var tokens = await GenerateTokensAsync(
            account,
            cancellationToken);

        return new TokenResult(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresAt);
    }
}