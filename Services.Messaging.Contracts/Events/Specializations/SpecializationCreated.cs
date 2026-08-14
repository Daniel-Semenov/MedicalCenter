using Services.Messaging.Contracts.Enums;

namespace Services.Messaging.Contracts.Events.Specializations;

public record SpecializationCreated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public SpecializationStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<SpecializationServiceItem> Services { get; init; } = Array.Empty<SpecializationServiceItem>();
}