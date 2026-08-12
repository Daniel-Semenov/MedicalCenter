namespace Appointments.Domain.Entities.Snapshots;

public class OfficeSnapshot : ISnapshotEntity
{
    public Guid Id { get; set; }
    public string Address { get; set; } = null!;
    public bool IsActive { get; set; }
}