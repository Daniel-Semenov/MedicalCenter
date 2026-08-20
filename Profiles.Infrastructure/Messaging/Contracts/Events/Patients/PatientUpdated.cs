namespace Profiles.Infrastructure.Messaging.Contracts.Events.Patients;

public record PatientUpdated
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; } 
    public string? MiddleName { get; init; }
    public DateTime? DateOfBirth { get; init; }
}