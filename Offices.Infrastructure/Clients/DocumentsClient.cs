using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Documents.Contracts.DTO_s;
using Microsoft.Extensions.Logging;
using Offices.Application.Interfaces;

namespace Offices.Infrastructure.Clients;

public class DocumentsClient : IDocumentsClient
{
    // Must stay in sync with Documents.Domain.Entities.DocumentOwnerType.OfficePhoto (= 1).
    // This is a cross-service contract that the compiler can't check — if that enum
    // in Documents ever changes numbering, this constant has to change too.
    private const int OfficePhotoOwnerType = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<DocumentsClient> _logger;

    public DocumentsClient(IHttpClientFactory httpClientFactory, ILogger<DocumentsClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("documents");
        _logger = logger;
    }

    public async Task<Guid> UploadOfficePhotoAsync(
        Guid officeId,
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();

        var streamContent = new StreamContent(content);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(streamContent, "File", fileName);
        form.Add(new StringContent(officeId.ToString()), "OwnerId");
        form.Add(new StringContent(OfficePhotoOwnerType.ToString()), "OwnerType");

        using var response = await _httpClient.PostAsync("api/documents/upload", form, cancellationToken);
        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiResultEnvelope<UploadDocumentResponseDto>>(
            JsonOptions, cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null)
        {
            throw new InvalidOperationException(
                $"Documents service rejected the upload: {envelope?.Error?.Message ?? "unknown error"}");
        }

        return envelope.Value.DocumentId;
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetPresignedUrlsAsync(
        IReadOnlyCollection<Guid> documentIds,
        CancellationToken cancellationToken = default)
    {
        if (documentIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var request = new GetPresignedUrlsRequestDto(documentIds.ToList());

        using var response = await _httpClient.PostAsJsonAsync(
            "api/documents/presigned-urls", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiResultEnvelope<List<PresignedUrlResponseDto>>>(
            JsonOptions, cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null)
        {
            _logger.LogWarning("Documents service returned no presigned URLs: {Error}", envelope?.Error?.Message);
            return new Dictionary<Guid, string>();
        }

        return envelope.Value.ToDictionary(x => x.DocumentId, x => x.Url);
    }

    public async Task DeleteAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/documents/{documentId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}