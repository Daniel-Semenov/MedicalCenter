namespace Profiles.Infrastructure.Messaging.Contracts.Events.Receptionist;

public record ReceptionistUpdated
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid? OfficeId { get; init; }
    public DateTime UpdatedAt { get; init; }
}