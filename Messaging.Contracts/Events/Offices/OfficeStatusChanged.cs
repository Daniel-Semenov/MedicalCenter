namespace Messaging.Contracts.Events.Offices;

public record OfficeStatusChanged
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
    public DateTime ChangedAt { get; init; }
}