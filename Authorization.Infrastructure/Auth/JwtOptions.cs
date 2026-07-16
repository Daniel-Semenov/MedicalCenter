namespace Authorization.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string SecretKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; }
    public int RefreshTokenExpirationHours { get; set; }
}