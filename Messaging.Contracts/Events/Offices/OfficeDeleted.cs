namespace Messaging.Contracts.Events.Offices;

public record OfficeDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}