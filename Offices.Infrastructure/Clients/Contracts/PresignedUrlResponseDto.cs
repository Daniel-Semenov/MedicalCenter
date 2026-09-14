namespace Offices.Infrastructure.Clients.Contracts;

internal sealed record PresignedUrlResponseDto(
    Guid DocumentId,
    string Url,
    DateTime ExpiresAt);