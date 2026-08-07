using Services.Messaging.Contracts.Enums;

namespace Services.Messaging.Contracts.Events.Services;

public record ServiceUpdated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
    public ServiceStatus Status { get; init; }
    public DateTime UpdatedAt { get; init; }
}