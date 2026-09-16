namespace Documents.Contracts.DTO_s;

public sealed record UploadDocumentResponseDto(
    Guid DocumentId,
    string FileName,
    string ContentType,
    long SizeBytes);