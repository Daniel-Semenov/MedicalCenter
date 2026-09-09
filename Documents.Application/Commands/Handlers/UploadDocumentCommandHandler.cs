using Documents.Application.DTO_s.Responses;
using Documents.Application.Interfaces;
using Documents.Application.Interfaces.Repositories;
using Documents.Domain.Entities;
using Documents.Shared.Common.Constants;
using Documents.Shared.Common.Errors;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Commands.Handlers;

public sealed class UploadDocumentCommandHandler
    : IRequestHandler<UploadDocumentCommand, Result<UploadDocumentResponse>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IObjectStorage _objectStorage;
    private readonly IObjectKeyGenerator _objectKeyGenerator;
    private readonly IObjectStorageOptions _storageOptions;

    public UploadDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IObjectStorage objectStorage,
        IObjectKeyGenerator objectKeyGenerator,
        IObjectStorageOptions storageOptions)
    {
        _documentRepository = documentRepository;
        _objectStorage = objectStorage;
        _objectKeyGenerator = objectKeyGenerator;
        _storageOptions = storageOptions;
    }

    public async Task<Result<UploadDocumentResponse>> Handle(
        UploadDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Content is null)
        {
            return Result<UploadDocumentResponse>.Failure(
                DocumentErrors.ContentRequired());
        }

        if (request.SizeBytes <= 0)
        {
            return Result<UploadDocumentResponse>.Failure(
                DocumentErrors.InvalidFileSize());
        }

        var documentId = Guid.NewGuid();

        var objectKey = _objectKeyGenerator.Generate(
            request.OwnerType,
            request.OwnerId,
            documentId,
            request.FileName);

        var document = new Document(
            documentId,
            request.OwnerType,
            request.OwnerId,
            _storageOptions.BucketName,
            objectKey,
            request.ContentType,
            request.FileName,
            request.SizeBytes);

        var uploaded = false;

        try
        {
            await _objectStorage.UploadAsync(
                StorageBuckets.Documents,
                document.ObjectKey,
                request.Content,
                request.ContentType,
                cancellationToken);

            uploaded = true;

            await _documentRepository.AddAsync(document, cancellationToken);

            await _documentRepository.SaveChangesAsync(cancellationToken);

            return Result<UploadDocumentResponse>.Success(
                new UploadDocumentResponse(
                    document.Id,
                    document.FileName,
                    document.ContentType,
                    document.SizeBytes));
        }
        catch (Exception exception) when (uploaded)
        {
                await _objectStorage.DeleteAsync(
                StorageBuckets.Documents,
                    document.ObjectKey,
                    CancellationToken.None);
                
                return Result<UploadDocumentResponse>
                    .Failure(DocumentErrors.DatabaseSaveFailed(exception.Message));
        }
        catch (Exception ex)
        {
            return Result<UploadDocumentResponse>.Failure(
                DocumentErrors.UploadFailed(ex.Message));
        }
    }
}