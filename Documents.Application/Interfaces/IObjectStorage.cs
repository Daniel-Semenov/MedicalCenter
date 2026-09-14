namespace Documents.Application.Interfaces;

public interface IObjectStorage
{
    Task<string> GetPresignedUrlAsync(
        string bucket,
        string objectKey,
        TimeSpan expiry,
        CancellationToken cancellationToken = default);
    Task UploadAsync(
        string bucket,
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(
        string bucket,
        string objectKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string bucket,
        string objectKey,
        CancellationToken cancellationToken = default);
}