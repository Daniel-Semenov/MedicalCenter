using Services.Messaging.Contracts.Enums;

namespace Services.Messaging.Contracts.Events.Services;

public record ServiceCreated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
    public ServiceCategory Category { get; init; }
    public ServiceStatus Status { get; init; }
    public Guid SpecializationId { get; init; }
    public DateTime CreatedAt { get; init; }
}