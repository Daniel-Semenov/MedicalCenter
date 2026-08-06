using Microsoft.AspNetCore.Http;

namespace Profiles.Shared.Common.Errors;

public static class PatientErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "PATIENT_NOT_FOUND",
            $"Patient with ID {id} not found.",
            StatusCodes.Status404NotFound);
    
    public static Error NotFound(string searchTerm) =>
        new(
            "PATIENT_NOT_FOUND",
            $"Patient with Name {searchTerm} not found.",
            StatusCodes.Status404NotFound);

    public static Error AccountNotFound(Guid accountId) =>
        new(
            "PATIENT_ACCOUNT_NOT_FOUND",
            $"Patient with account ID {accountId} not found.",
            StatusCodes.Status404NotFound);

    public static Error CreateFailed(string message) =>
        new(
            "PATIENT_CREATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);

    public static Error UpdateFailed(string message) =>
        new(
            "PATIENT_UPDATE_FAILED",
            message,
            StatusCodes.Status400BadRequest);
}