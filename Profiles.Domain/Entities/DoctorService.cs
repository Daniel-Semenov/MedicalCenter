namespace Profiles.Domain.Entities;

public class DoctorService
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public Doctor? Doctor { get; set; }
}