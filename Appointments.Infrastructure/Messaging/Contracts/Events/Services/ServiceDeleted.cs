namespace Services.Messaging.Contracts.Events.Services;

public record ServiceDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}