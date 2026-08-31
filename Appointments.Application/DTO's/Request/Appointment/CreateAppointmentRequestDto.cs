namespace Appointments.Application.DTO_s.Request.Appointment;

public class CreateAppointmentRequestDto
{
    public Guid PatientId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid TimeSlotId { get; set; }
}