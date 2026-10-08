namespace Appointments.Domain.Entities;

public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public Guid OfficeId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSlotStatus Status { get; set; }
    public Guid? AppointmentId { get; set; }
    public uint RowVersion { get; set; }
}