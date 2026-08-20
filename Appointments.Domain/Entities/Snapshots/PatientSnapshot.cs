namespace Appointments.Domain.Entities.Snapshots;

public class PatientSnapshot : ISnapshotEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } 
    
}