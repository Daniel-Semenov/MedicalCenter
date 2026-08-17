namespace Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

public record DoctorUpdated
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid? SpecializationId { get; init; }
    public Guid? OfficeId { get; init; }
    public int CareerStartYear { get; init; }
    public DateTime UpdatedAt { get; init; }
}