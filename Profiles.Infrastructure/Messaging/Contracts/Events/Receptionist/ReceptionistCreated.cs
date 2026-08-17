namespace Profiles.Infrastructure.Messaging.Contracts.Events.Receptionist;

public record ReceptionistCreated
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid? OfficeId { get; init; }
    public DateTime CreatedAt { get; init; }
}