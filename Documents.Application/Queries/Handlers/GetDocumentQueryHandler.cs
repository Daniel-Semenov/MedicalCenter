using Documents.Application.DTO_s.Responses;
using Documents.Application.Interfaces.Repositories;
using Documents.Shared.Common.Errors;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Queries.Handlers;

public sealed class GetDocumentQueryHandler
    : IRequestHandler<GetDocumentQuery, Result<DocumentResponse>>
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentQueryHandler(
        IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Result<DocumentResponse>> Handle(
        GetDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            request.DocumentId,
            cancellationToken);

        if (document is null || document.IsDeleted)
        {
            return Result<DocumentResponse>.Failure(DocumentErrors.NotFound(request.DocumentId));
        }

        return Result<DocumentResponse>.Success(
            new DocumentResponse(
                document.Id,
                document.OwnerType,
                document.OwnerId,
                document.FileName,
                document.ContentType,
                document.SizeBytes,
                document.CreatedAt));
    }
}