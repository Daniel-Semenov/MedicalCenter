using Services.Messaging.Contracts.Enums;

namespace Services.Messaging.Contracts.Events.Specializations;

public record SpecializationServiceItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
    public ServiceCategory Category { get; init; }
    public ServiceStatus Status { get; init; }
}