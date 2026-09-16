using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Appointments.Application.Interfaces;
using Documents.Contracts;
using Documents.Contracts.DTO_s;


namespace Appointments.Infrastructure.Clients;

public class DocumentsClient : IDocumentsClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public DocumentsClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("documents");
    }

    public async Task<Guid> UploadAppointmentResultPdfAsync(
        Guid appointmentResultId,
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();

        var byteContent = new ByteArrayContent(content);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        form.Add(byteContent, "File", fileName);
        form.Add(new StringContent(appointmentResultId.ToString()), "OwnerId");
        form.Add(new StringContent(((int)DocumentOwnerType.AppointmentResultPdf).ToString()), "OwnerType");

        using var response = await _httpClient.PostAsync("api/documents/upload", form, cancellationToken);
        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiResultEnvelope<UploadDocumentResponseDto>>(
            JsonOptions, cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null)
        {
            throw new InvalidOperationException(
                $"Documents service rejected the PDF upload: {envelope?.Error?.Message ?? "unknown error"}");
        }

        return envelope.Value.DocumentId;
    }

    public async Task<byte[]> DownloadAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/documents/{documentId}/download", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"api/documents/{documentId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}