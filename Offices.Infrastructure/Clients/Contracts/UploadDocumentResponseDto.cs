namespace Offices.Infrastructure.Clients.Contracts;

internal sealed record UploadDocumentResponseDto(
    Guid DocumentId,
    string FileName,
    string ContentType,
    long SizeBytes);