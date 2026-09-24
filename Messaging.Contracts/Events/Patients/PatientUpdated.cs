namespace Messaging.Contracts.Events.Patients;

public record PatientUpdated
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}