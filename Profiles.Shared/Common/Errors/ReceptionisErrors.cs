using Microsoft.AspNetCore.Http;

namespace Profiles.Shared.Common.Errors;

public static class ReceptionistErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "RECEPTIONIST_NOT_FOUND",
            $"Receptionist with ID {id} not found.",
            StatusCodes.Status404NotFound);

    public static Error AccountNotFound(Guid accountId) =>
        new(
            "RECEPTIONIST_ACCOUNT_NOT_FOUND",
            $"Account with ID {accountId} not found.",
            StatusCodes.Status404NotFound);

    public static Error CreateFailed(string message) =>
        new(
            "RECEPTIONIST_CREATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error UpdateFailed(string message) =>
        new(
            "RECEPTIONIST_UPDATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);
}