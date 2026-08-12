namespace Appointments.Application.DTO_s.Request.Appointment;

public class RescheduleAppointmentRequestDto
{
    public DateTime NewAppointmentDate { get; set; }
    public DateTime NewStartTime { get; set; }
}