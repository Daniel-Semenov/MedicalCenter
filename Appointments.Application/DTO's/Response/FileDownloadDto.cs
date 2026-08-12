namespace Appointments.Application.DTO_s.Response;

public class FileDownloadDto
{
    public byte[] Content { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = "application/pdf";
}