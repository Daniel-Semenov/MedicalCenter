using Appointments.Domain.Entities;

namespace Appointments.Application.DTO_s.Response.Appointment;

public class AppointmentDetailsDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid SpecializationId { get; set; }
    public Guid OfficeId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string DoctorFullName { get; set; } = null!;
    public string PatientFullName { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public string SpecializationName { get; set; } = null!;
    public string OfficeAddress { get; set; } = null!;
}