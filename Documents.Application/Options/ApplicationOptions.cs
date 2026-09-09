namespace Documents.Application.Options;

public class ApplicationOptions
{
    public long MaxFileSizeBytes { get; set; }
    public string[] AllowedContentTypes { get; set; } = Array.Empty<string>();
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
}