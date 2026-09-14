using Documents.Application.DTO_s.Responses;
using Documents.Application.Interfaces;
using Documents.Application.Interfaces.Repositories;
using Documents.Shared.Common.Constants;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Queries.Handlers;

public sealed class GetPresignedUrlsQueryHandler
    : IRequestHandler<GetPresignedUrlsQuery, Result<IReadOnlyList<PresignedUrlResponse>>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IObjectStorage _objectStorage;

    public GetPresignedUrlsQueryHandler(
        IDocumentRepository documentRepository,
        IObjectStorage objectStorage)
    {
        _documentRepository = documentRepository;
        _objectStorage = objectStorage;
    }

    public async Task<Result<IReadOnlyList<PresignedUrlResponse>>> Handle(
        GetPresignedUrlsQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetByIdsAsync(request.DocumentIds, cancellationToken);

        var expiry = TimeSpan.FromMinutes(PresignedUrlConstants.ExpiryMinutes);
        var expiresAt = DateTime.UtcNow.Add(expiry);

        var responses = new List<PresignedUrlResponse>(documents.Count);

        foreach (var document in documents)
        {
            var url = await _objectStorage.GetPresignedUrlAsync(
                document.Bucket,
                document.ObjectKey,
                expiry,
                cancellationToken);

            responses.Add(new PresignedUrlResponse(document.Id, url, expiresAt));
        }

        return Result<IReadOnlyList<PresignedUrlResponse>>.Success(responses);
    }
}