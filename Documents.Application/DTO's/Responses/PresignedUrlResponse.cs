namespace Documents.Application.DTO_s.Responses;

public sealed record PresignedUrlResponse(
    Guid DocumentId,
    string Url,
    DateTime ExpiresAt);