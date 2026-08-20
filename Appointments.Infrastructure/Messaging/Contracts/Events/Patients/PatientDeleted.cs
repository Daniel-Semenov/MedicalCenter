namespace Profiles.Infrastructure.Messaging.Contracts.Events.Patients;

public record PatientDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}