namespace Notifications.Application.DTO_s;

public sealed class DocumentFile
{
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public byte[] Content { get; init; } = null!;
}