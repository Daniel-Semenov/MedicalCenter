namespace Appointments.Domain.Entities.Snapshots;

public class SpecializationSnapshot : ISnapshotEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}