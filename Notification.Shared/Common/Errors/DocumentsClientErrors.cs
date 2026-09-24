using System.Net;
using Microsoft.AspNetCore.Http;

namespace Notification.Shared.Common.Errors;

public static class DocumentsClientErrors
{
    public static Error DownloadFailed(
        Guid documentId,
        HttpStatusCode statusCode) => new Error(
            "DOCUMENTS_DOWNLOAD_FAILED",
            $"Failed to download document {documentId}.",
           StatusCodes.Status400BadRequest);
}