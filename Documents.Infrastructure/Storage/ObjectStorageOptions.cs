using Documents.Application.Interfaces;

namespace Documents.Infrastructure.Storage;

public sealed class ObjectStorageOptions
    : IObjectStorageOptions
{
    public string BucketName { get; }

    public ObjectStorageOptions(string bucketName)
    {
        BucketName = bucketName;
    }
}