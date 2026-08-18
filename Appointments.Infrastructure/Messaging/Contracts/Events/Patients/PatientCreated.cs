namespace Profiles.Infrastructure.Messaging.Contracts.Events.Patients;

public record PatientCreated
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}