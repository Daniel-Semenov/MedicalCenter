namespace Messaging.Contracts.Events.Receptionist;

public record ReceptionistDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}