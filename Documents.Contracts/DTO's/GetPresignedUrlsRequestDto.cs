namespace Documents.Contracts.DTO_s;

public sealed record GetPresignedUrlsRequestDto(
    IReadOnlyList<Guid> DocumentIds);