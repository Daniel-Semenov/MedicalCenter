using Appointments.Infrastructure.Messaging.Contracts.Enums;

namespace Appointments.Infrastructure.Messaging.Contracts.Services;

public record ServiceCreated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid SpecializationId { get; init; }
    public ServiceStatusSnapshot Status { get; init; }
    public DateTime CreatedAt { get; init; }
}