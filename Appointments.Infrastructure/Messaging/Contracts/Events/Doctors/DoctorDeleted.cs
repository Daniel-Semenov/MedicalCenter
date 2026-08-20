namespace Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

public record DoctorDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}