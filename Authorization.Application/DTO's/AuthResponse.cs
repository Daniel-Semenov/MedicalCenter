namespace Authorization.Application.DTO_s;


public record AuthResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);