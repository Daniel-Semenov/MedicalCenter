namespace Profiles.Infrastructure.Messaging.Contracts.Events.Patients;

public record PatientCreated
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public bool IsLinkedToAccount { get; init; }
    public DateTime CreatedAt { get; init; }
}