using Documents.Application.Interfaces;
using Documents.Application.Interfaces.Repositories;
using Documents.Shared.Common.Constants;
using Documents.Shared.Common.Errors;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Commands.Handlers;

public sealed class DeleteDocumentCommandHandler
    : IRequestHandler<DeleteDocumentCommand, Result>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IObjectStorage _objectStorage;

    public DeleteDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IObjectStorage objectStorage)
    {
        _documentRepository = documentRepository;
        _objectStorage = objectStorage;
    }

    public async Task<Result> Handle(
        DeleteDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            request.DocumentId,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure(DocumentErrors.NotFound(request.DocumentId));
        }

        if (document.IsDeleted)
        {
            return Result.Success();
        }

        
        await _objectStorage.DeleteAsync(
            StorageBuckets.Documents,
            document.ObjectKey,
            cancellationToken);

        document.MarkAsDeleted();

        await _documentRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}