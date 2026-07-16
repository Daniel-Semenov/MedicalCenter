using Microsoft.AspNetCore.Http;

namespace Authorization.Shared.Common.Errors;

public static class AccountErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "ACCOUNT_NOT_FOUND",
            $"Account with ID {id} not found.",
            StatusCodes.Status404NotFound);

    public static Error EmailNotFound(string email) =>
        new(
            "ACCOUNT_EMAIL_NOT_FOUND",
            $"Account with email {email} not found.",
            StatusCodes.Status404NotFound);

    public static Error CreateFailed(string message) =>
        new(
            "ACCOUNT_CREATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);
    
    public static Error UpdateFailed(string message) =>
        new(
            "ACCOUNT_UPDATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);
    public static Error DeleteFailed(string message) =>
        new(
            "ACCOUNT_DELETE_FAILED",
            message,
            StatusCodes.Status400BadRequest);
}