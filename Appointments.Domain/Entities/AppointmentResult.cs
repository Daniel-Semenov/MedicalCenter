namespace Appointments.Domain.Entities;

public class AppointmentResult
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string Complaints { get; set; } = null!;
    public string Conclusion { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
    public Guid? PdfDocumentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}