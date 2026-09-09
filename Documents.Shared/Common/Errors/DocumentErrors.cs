using Microsoft.AspNetCore.Http;

namespace Documents.Shared.Common.Errors;

public static class DocumentErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "DOCUMENT_NOT_FOUND",
            $"Document with ID {id} not found.",
            StatusCodes.Status404NotFound);
    
    public static Error ContentRequired() =>
        new(
            "DOCUMENT_CONTENT_REQUIRED",
            "Document content is required.",
            StatusCodes.Status400BadRequest);

    public static Error InvalidFileSize() =>
        new(
            "DOCUMENT_INVALID_FILE_SIZE",
            "Document size must be greater than zero.",
            StatusCodes.Status400BadRequest);
    
    public static Error UploadFailed(string? details = null) =>
        new(
            "DOCUMENT_UPLOAD_FAILED",
            $"Failed to upload document to storage.{(string.IsNullOrEmpty(details) ? "" : $" Details: {details}")}",
            StatusCodes.Status500InternalServerError);

    public static Error DatabaseSaveFailed(string? details = null) =>
        new(
            "DOCUMENT_DATABASE_SAVE_FAILED",
            $"Failed to save document metadata.{(string.IsNullOrEmpty(details) ? "" : $" Details: {details}")}",
            StatusCodes.Status500InternalServerError);
}