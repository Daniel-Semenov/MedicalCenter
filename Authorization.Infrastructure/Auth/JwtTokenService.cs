using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Infrastructure.Auth;

public class JwtTokenService : ITokenService
{
    private readonly RefreshTokenGenerator _refreshGenerator;
    private readonly RefreshTokenService _refreshService;
    private readonly JwtOptions _options;


    public JwtTokenService(
        IOptions<JwtOptions> options, RefreshTokenGenerator refreshGenerator, RefreshTokenService refreshService)
    {
        _refreshGenerator = refreshGenerator;
        _refreshService = refreshService;
        _options = options.Value;
    }


    public async Task<TokenResult> GenerateTokensAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                account.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                account.Email!)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _options.SecretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var expiresAt =
            DateTime.UtcNow.AddMinutes(
                _options.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);
        
        var refreshToken =
            _refreshGenerator.Generate();

        await _refreshService.SaveAsync(
            account.Id,
            refreshToken,
            TimeSpan.FromHours(
                _options.RefreshTokenExpirationHours));

        return await Task.FromResult(
            new TokenResult(
                accessToken,
                refreshToken,
                expiresAt));
    }
}