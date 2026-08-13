namespace Appointments.Infrastructure.Messaging.Contracts.Specializations;

public record SpecializationDeleted
{
    public Guid Id { get; init; }
    public DateTime DeletedAt { get; init; }
}