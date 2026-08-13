using Appointments.Infrastructure.Messaging.Contracts.Enums;

namespace Appointments.Infrastructure.Messaging.Contracts.Specializations;

public record SpecializationServiceItemSnapshot
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public ServiceStatusSnapshot Status { get; init; }
}