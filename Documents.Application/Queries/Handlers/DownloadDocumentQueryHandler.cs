using Documents.Application.DTO_s.Responses;
using Documents.Application.Interfaces;
using Documents.Application.Interfaces.Repositories;
using Documents.Shared.Common.Constants;
using Documents.Shared.Common.Errors;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Queries.Handlers;

public sealed class DownloadDocumentQueryHandler
    : IRequestHandler<
        DownloadDocumentQuery,
        Result<DocumentDownloadResponse>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IObjectStorage _objectStorage;

    public DownloadDocumentQueryHandler(
        IDocumentRepository documentRepository,
        IObjectStorage objectStorage)
    {
        _documentRepository = documentRepository;
        _objectStorage = objectStorage;
    }

    public async Task<Result<DocumentDownloadResponse>> Handle(
        DownloadDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            request.DocumentId,
            cancellationToken);

        if (document is null || document.IsDeleted)
        {
            return Result<DocumentDownloadResponse>.Failure(
                DocumentErrors.NotFound(request.DocumentId));
        }

        var content = await _objectStorage.DownloadAsync(
            StorageBuckets.Documents,
            document.ObjectKey,
            cancellationToken);

        return Result<DocumentDownloadResponse>.Success(
            new DocumentDownloadResponse(
                content,
                document.FileName,
                document.ContentType,
                document.SizeBytes));
    }
}