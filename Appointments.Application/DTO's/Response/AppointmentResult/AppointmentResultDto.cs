namespace Appointments.Application.DTO_s.Response.AppointmentResult;

public class AppointmentResultDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public DateTime ResultDate { get; set; }      
    public string PatientFullName { get; set; } = null!;
    public string PatientEmail { get; set; } = null!;
    public DateTime? PatientDateOfBirth { get; set; }
    public string DoctorFullName { get; set; } = null!;
    public string DoctorSpecialization { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public string Complaints { get; set; } = null!;
    public string Conclusion { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}