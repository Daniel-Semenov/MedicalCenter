using Microsoft.AspNetCore.Http;

namespace Authorization.Shared.Common.Errors;

public static class AuthErrors
{
    public static Error InvalidCredentials =>
        new(
            "AUTH_INVALID_CREDENTIALS",
            "Invalid email or password.",
            StatusCodes.Status401Unauthorized);

    public static Error EmailAlreadyExists(string email) =>
        new(
            "AUTH_EMAIL_ALREADY_EXISTS",
            $"Account with email {email} already exists.",
            StatusCodes.Status409Conflict);

    public static Error RegistrationFailed(string message) =>
        new(
            "AUTH_REGISTRATION_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error LogoutFailed(string message) =>
        new(
            "AUTH_LOGOUT_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error InvalidRefreshToken(string refreshToken) =>
        new(
            "AUTH_INVALID_REFRESH_TOKEN",
            $"Invalid refresh token: {refreshToken}",
            StatusCodes.Status400BadRequest
        );
    
    public static Error UserNotFound =>
        new(
            "USER_NOT_FOUND",
            "User was not found.",
            StatusCodes.Status404NotFound);

    public static Error RoleCreationFailed(string message) =>
        new(
            "ROLE_CREATION_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error RoleAssignmentFailed(string message) =>
        new(
            "ROLE_ASSIGNMENT_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error UserAlreadyHasRole =>
        new(
            "ROLE_ALREADY_EXISTS",
            "User already has role.",
            StatusCodes.Status400BadRequest);

    public static Error RoleNotFound => 
        new(
        "ROLE_NOT_FOUND", 
        "User role was not found.",
        StatusCodes.Status404NotFound);
}