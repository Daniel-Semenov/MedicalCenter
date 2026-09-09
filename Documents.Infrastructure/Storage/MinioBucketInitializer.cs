using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Options;

namespace Documents.Infrastructure.Storage;

public sealed class MinioBucketInitializer
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioBucketInitializer(
        IMinioClient minioClient,
        IOptions<MinioOptions> options)  
    {
        _minioClient = minioClient;
        _options = options.Value;      
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_options.BucketName);

        var exists = await _minioClient.BucketExistsAsync(
            bucketExistsArgs,
            cancellationToken);

        if (exists)
        {
            return;
        }

        var makeBucketArgs = new MakeBucketArgs()
            .WithBucket(_options.BucketName);

        await _minioClient.MakeBucketAsync(
            makeBucketArgs,
            cancellationToken);
    }
}