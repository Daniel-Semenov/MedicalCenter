namespace Messaging.Contracts.Events.Doctors;

public record DoctorScheduleUpdated
{
    public Guid DoctorId { get; init; }
    public IReadOnlyCollection<DoctorScheduleEntry> Entries { get; init; } = Array.Empty<DoctorScheduleEntry>();
}