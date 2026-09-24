namespace Profiles.Application.DTO_s.Request;

public class UpdatePatientRequestDto
{
    public bool IsLinkedToAccount { get; set; }
    public string Email { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}