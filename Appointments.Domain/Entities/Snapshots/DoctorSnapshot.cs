namespace Appointments.Domain.Entities.Snapshots;

public class DoctorSnapshot : ISnapshotEntity
{
    public Guid Id { get; set; } 
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public bool IsAtWork { get; set; } 
}