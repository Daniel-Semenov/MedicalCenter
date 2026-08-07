using Services.Messaging.Contracts.Enums;
using Services.Messaging.Contracts.Events.Specializations;

namespace Services.Messaging.Contracts.Events;

public record SpecializationCreated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public SpecializationStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<SpecializationServiceItem> Services { get; init; } = Array.Empty<SpecializationServiceItem>();
}