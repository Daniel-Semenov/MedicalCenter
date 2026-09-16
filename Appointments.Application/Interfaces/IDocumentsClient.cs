namespace Appointments.Application.Interfaces;

public interface IDocumentsClient
{
    Task<Guid> UploadAppointmentResultPdfAsync(
        Guid appointmentResultId,
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<byte[]> DownloadAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}