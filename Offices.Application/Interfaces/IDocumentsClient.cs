namespace Offices.Application.Interfaces;

public interface IDocumentsClient
{
    Task<Guid> UploadOfficePhotoAsync(
        Guid officeId,
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, string>> GetPresignedUrlsAsync(
        IReadOnlyCollection<Guid> documentIds,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}