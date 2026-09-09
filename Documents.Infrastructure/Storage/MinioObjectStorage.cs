using Documents.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;

namespace Documents.Infrastructure.Storage;

public sealed class MinioObjectStorage : IObjectStorage
{
    private readonly IMinioClient _minioClient;

    public MinioObjectStorage(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task UploadAsync(
        string bucket,
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var args = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(
            args,
            cancellationToken);
    }

    public async Task<Stream> DownloadAsync(
        string bucket,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            });

        await _minioClient.GetObjectAsync(
            args,
            cancellationToken);

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task DeleteAsync(
        string bucket,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(
            args,
            cancellationToken);
    }
}