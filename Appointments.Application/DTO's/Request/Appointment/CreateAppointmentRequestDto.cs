namespace Appointments.Application.DTO_s.Request.Appointment;

public class CreateAppointmentRequestDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid OfficeId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
}