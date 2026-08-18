using Services.Messaging.Contracts.Events.Enums;

namespace Services.Messaging.Contracts.Events.Specializations;


public record SpecializationServiceItemSnapshot
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public ServiceStatusSnapshot Status { get; init; }
}