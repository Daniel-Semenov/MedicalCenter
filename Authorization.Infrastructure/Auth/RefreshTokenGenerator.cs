using System.Security.Cryptography;

namespace Authorization.Infrastructure.Auth;

public class RefreshTokenGenerator
{
    public string Generate()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}