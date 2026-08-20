using Messaging.Contracts.Events.Enums;

namespace Messaging.Contracts.Events.Services;

public record ServiceUpdated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid SpecializationId { get; init; }
    public ServiceStatusSnapshot Status { get; init; }
    public DateTime UpdatedAt { get; init; }
}