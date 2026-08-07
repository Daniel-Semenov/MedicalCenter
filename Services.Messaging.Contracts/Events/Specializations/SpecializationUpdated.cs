using Services.Messaging.Contracts.Enums;

namespace Services.Messaging.Contracts.Events.Specializations;

public record SpecializationUpdated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public SpecializationStatus Status { get; init; }
    public DateTime UpdatedAt { get; init; }
}