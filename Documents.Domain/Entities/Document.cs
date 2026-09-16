using Documents.Contracts;

namespace Documents.Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }

    public DocumentOwnerType OwnerType { get; private set; }

    public Guid OwnerId { get; private set; }

    public string Bucket { get; private set; } = null!;

    public string ObjectKey { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;

    public string FileName { get; private set; } = null!;

    public long SizeBytes { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    private Document()
    {
    }

    public Document(
        Guid id,
        DocumentOwnerType ownerType,
        Guid ownerId,
        string bucket,
        string objectKey,
        string contentType,
        string fileName,
        long sizeBytes)
    {
        Id = id;
        OwnerType = ownerType;
        OwnerId = ownerId;
        Bucket = bucket;
        ObjectKey = objectKey;
        ContentType = contentType;
        FileName = fileName;
        SizeBytes = sizeBytes;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}