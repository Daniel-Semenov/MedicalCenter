namespace Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

public record DoctorCreated
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid? SpecializationId { get; init; }
    public Guid? OfficeId { get; init; }
    public int CareerStartYear { get; init; }
    public DateTime CreatedAt { get; init; }
}