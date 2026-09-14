using Documents.Application.DTO_s.Responses;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Queries;

public sealed record GetPresignedUrlsQuery(
    IReadOnlyList<Guid> DocumentIds) : IRequest<Result<IReadOnlyList<PresignedUrlResponse>>>;