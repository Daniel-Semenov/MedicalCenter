namespace Messaging.Contracts.Events.AppointmentResults;

public record AppointmentResultCreated
{
    public Guid Id { get; init; }
    public Guid AppointmentResultId { get; init; }
    public Guid AppointmentId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientEmail { get; init; } = null!;
    public string PatientFullName { get; init; } = null!;
    public DateTime ResultDate { get; init; }
    public string DoctorFullName { get; init; } = null!;
    public string DoctorSpecialization { get; init; } = null!;
    public string ServiceName { get; init; } = null!;
    public Guid PdfDocumentId { get; init; }
    public DateTime CreatedAt { get; init; }
}
