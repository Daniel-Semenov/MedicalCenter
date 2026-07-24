using MediatR;
using Offices.Application.Interfaces;

namespace Offices.Application.Commands.Handlers;

public class UpdateOfficePhotoCommandHandler : IRequestHandler<UpdateOfficePhotoCommand>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IFileStorageService _fileStorageService;

    public UpdateOfficePhotoCommandHandler(
        IOfficeRepository officeRepository,
        IFileStorageService fileStorageService)
    {
        _officeRepository = officeRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(UpdateOfficePhotoCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        
        if (office is null)
        {
            throw new KeyNotFoundException($"Office with id {request.OfficeId} not found"); 
        }

        if (!string.IsNullOrEmpty(office.PhotoUrl))
        {
            await _fileStorageService.DeleteFileAsync(office.PhotoUrl, cancellationToken);
        }

        var photoUrl = await _fileStorageService.UploadFileAsync(
            request.PhotoStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        office.UpdatePhoto(photoUrl);
        await _officeRepository.UpdateAsync(office, cancellationToken);
    }
}