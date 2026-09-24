using Notification.Shared.Common.Errors;
using Notification.Shared.Common.Results;
using Notifications.Application.DTO_s;
using Notifications.Application.Interfaces;

namespace Notifications.Infrastructure.Clients;

public sealed class DocumentsClient : IDocumentsClient
{
    private readonly HttpClient _httpClient;

    public DocumentsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<DocumentFile>> DownloadAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"api/documents/{documentId}/download",
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result<DocumentFile>.Failure(
                DocumentsClientErrors.DownloadFailed(
                    documentId,
                    response.StatusCode));
        }

        var content = await response.Content.ReadAsByteArrayAsync(
            cancellationToken);

        var contentType =
            response.Content.Headers.ContentType?.MediaType
            ?? "application/octet-stream";

        var fileName =
            response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
            ?? "document";

        return Result<DocumentFile>.Success(
            new DocumentFile
            {
                FileName = fileName,
                ContentType = contentType,
                Content = content
            });
    }
}