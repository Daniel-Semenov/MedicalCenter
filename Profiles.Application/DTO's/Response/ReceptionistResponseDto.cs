namespace Profiles.Application.DTO_s.Response;

public class ReceptionistResponseDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? OfficeId { get; set; }
    public string? PhotoUrl { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? OfficeName { get; set; }
    public string? OfficeAddress { get; set; }
}