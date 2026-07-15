using Microsoft.AspNetCore.Http;

namespace Profiles.Shared.Common.Errors;

public static class DoctorErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "DOCTOR_NOT_FOUND",
            $"Doctor with ID {id} not found.",
            StatusCodes.Status404NotFound);

    public static Error AccountNotFound(Guid accountId) =>
        new(
            "DOCTOR_ACCOUNT_NOT_FOUND",
            $"Doctor with account ID {accountId} not found.",
            StatusCodes.Status404NotFound);

    public static Error CreateFailed(string message) =>
        new(
            "DOCTOR_CREATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error UpdateFailed(string message) =>
        new(
            "DOCTOR_UPDATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);
}