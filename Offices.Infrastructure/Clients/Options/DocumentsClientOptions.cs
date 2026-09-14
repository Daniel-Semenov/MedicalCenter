namespace Offices.Infrastructure.Clients.Options;

public class DocumentsClientOptions
{
    public const string SectionName = "Documents";

    public string BaseUrl { get; set; } = null!;
}