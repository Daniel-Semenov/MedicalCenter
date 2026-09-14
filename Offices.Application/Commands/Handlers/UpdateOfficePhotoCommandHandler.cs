using MediatR;
using Microsoft.Extensions.Logging;
using Offices.Application.Interfaces;
using Offices.Domain.Exceptions;

namespace Offices.Application.Commands.Handlers;

public class UpdateOfficePhotoCommandHandler : IRequestHandler<UpdateOfficePhotoCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IDocumentsClient _documentsClient;
    private readonly ILogger<UpdateOfficePhotoCommandHandler> _logger;

    public UpdateOfficePhotoCommandHandler(
        IOfficeRepository officeRepository,
        IDocumentsClient documentsClient,
        ILogger<UpdateOfficePhotoCommandHandler> logger)
    {
        _officeRepository = officeRepository;
        _documentsClient = documentsClient;
        _logger = logger;
    }

    public async Task Handle(UpdateOfficePhotoCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);

        if (office is null)
        {
            throw new OfficeNotFoundException($"Office with id {request.OfficeId} not found");
        }

        var previousDocumentId = office.PhotoDocumentId;

        var newDocumentId = await _documentsClient.UploadOfficePhotoAsync(
            office.Id,
            request.PhotoStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        try
        {
            office.UpdatePhoto(newDocumentId);
            await _officeRepository.UpdateAsync(office, cancellationToken);
        }
        catch
        {
            try
            {
                await _documentsClient.DeleteAsync(newDocumentId, CancellationToken.None);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception,
                    "Failed to clean up orphaned document {DocumentId} for office {OfficeId} after a failed photo update.",
                    newDocumentId, office.Id);
            }

            throw;
        }

        if (previousDocumentId.HasValue)
        {
            try
            {
                await _documentsClient.DeleteAsync(previousDocumentId.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to delete previous document {DocumentId} for office {OfficeId}. It is now orphaned in Documents.",
                    previousDocumentId.Value, office.Id);
            }
        }
    }
}