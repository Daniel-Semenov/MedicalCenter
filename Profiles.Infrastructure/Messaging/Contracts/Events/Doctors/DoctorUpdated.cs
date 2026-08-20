namespace Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

public record DoctorUpdated
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; } 
    public string? MiddleName { get; init; }
    public Guid? SpecializationId { get; init; }
    public Guid? OfficeId { get; init; }
    public int CareerStartYear { get; init; }
}