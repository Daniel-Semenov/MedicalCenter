using Appointments.Domain.Entities;

namespace Appointments.Application.DTO_s.Response.Appointment;

public class AppointmentListItemDto
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string DoctorFullName { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public AppointmentStatus Status { get; set; }
    public Guid? ResultId { get; set; }      
    public bool CanReschedule { get; set; } 
}