namespace Authorization.Application.DTO_s;

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);