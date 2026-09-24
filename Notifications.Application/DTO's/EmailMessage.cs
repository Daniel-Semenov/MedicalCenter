namespace Notifications.Application.DTO_s;

public sealed class EmailMessage
{
    public string To { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public string HtmlBody { get; init; } = null!;
    public IReadOnlyCollection<DocumentFile> Attachments { get; init; } = [];
}