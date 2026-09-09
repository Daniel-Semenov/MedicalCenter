namespace Documents.Application.DTO_s.Responses;

public sealed record UploadDocumentResponse(
    Guid DocumentId,
    string FileName,
    string ContentType,
    long SizeBytes);