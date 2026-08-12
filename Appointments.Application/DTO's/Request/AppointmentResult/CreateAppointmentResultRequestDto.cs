namespace Appointments.Application.DTO_s.Request.AppointmentResult;

public class CreateAppointmentResultRequestDto
{
    public Guid AppointmentId { get; set; }
    public string Complaints { get; set; } = null!;
    public string Conclusion { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
}