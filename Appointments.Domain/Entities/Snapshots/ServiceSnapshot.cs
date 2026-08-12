namespace Appointments.Domain.Entities.Snapshots;

public class ServiceSnapshot : ISnapshotEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid SpecializationId { get; set; }
    public bool IsActive { get; set; }
}