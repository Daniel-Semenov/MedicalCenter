namespace Documents.Application.DTO_s.Requests;

public sealed record GetPresignedUrlsRequest(
    IReadOnlyList<Guid> DocumentIds);