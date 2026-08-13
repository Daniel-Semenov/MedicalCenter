using Appointments.Infrastructure.Messaging.Contracts.Enums;

namespace Appointments.Infrastructure.Messaging.Contracts.Specializations;


public record SpecializationUpdated
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public SpecializationStatusSnapshot Status { get; init; }
    public DateTime UpdatedAt { get; init; }
}