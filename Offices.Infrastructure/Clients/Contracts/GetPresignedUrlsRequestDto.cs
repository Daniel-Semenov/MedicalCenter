namespace Offices.Infrastructure.Clients.Contracts;


internal sealed record GetPresignedUrlsRequestDto(
    IReadOnlyList<Guid> DocumentIds);