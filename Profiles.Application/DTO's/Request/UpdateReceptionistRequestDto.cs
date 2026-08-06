namespace Profiles.Application.DTO_s.Request;

public class UpdateReceptionistRequestDto
{
    public Guid? OfficeId { get; set; }
    public string? PhotoUrl { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? OfficeName { get; set; }
    public string? OfficeAddress { get; set; }
}