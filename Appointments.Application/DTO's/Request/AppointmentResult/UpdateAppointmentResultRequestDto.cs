namespace Appointments.Application.DTO_s.Request.AppointmentResult;

public class UpdateAppointmentResultRequestDto
{
    public string Complaints { get; set; } = null!;
    public string Conclusion { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
}