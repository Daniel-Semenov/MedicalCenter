using Appointments.Infrastructure.Messaging.Contracts.Enums;

namespace Appointments.Infrastructure.Messaging.Contracts.Specializations;

public record SpecializationCreated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public SpecializationStatusSnapshot Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<SpecializationServiceItemSnapshot> Services { get; init; } = Array.Empty<SpecializationServiceItemSnapshot>();
}