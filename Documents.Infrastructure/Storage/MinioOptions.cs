namespace Documents.Infrastructure.Storage;

public sealed class MinioOptions
{
    public const string SectionName = "MinIO";

    public string Endpoint { get; set; } = null!;

    public string AccessKey { get; set; } = null!;

    public string SecretKey { get; set; } = null!;

    public bool UseSsl { get; set; }

    public string BucketName { get; set; } = "documents";
}