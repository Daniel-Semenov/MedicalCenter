namespace Documents.Contracts.DTO_s;

public sealed record PresignedUrlResponseDto(
    Guid DocumentId,
    string Url,
    DateTime ExpiresAt);