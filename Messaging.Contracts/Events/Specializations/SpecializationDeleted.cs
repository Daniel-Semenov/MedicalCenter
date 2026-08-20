namespace Messaging.Contracts.Events.Specializations;

public record SpecializationDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}