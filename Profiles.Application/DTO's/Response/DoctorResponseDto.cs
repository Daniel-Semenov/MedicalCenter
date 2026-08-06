namespace Profiles.Application.DTO_s.Response;

public class DoctorResponseDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public int CareerStartYear { get; set; }
    public int Experience { get; set; }
    public string? PhotoUrl { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? OfficeName { get; set; }
    public string? OfficeAddress { get; set; }
    public string? SpecializationName { get; set; }
    public List<DoctorServiceResponseDto> Services { get; set; } = new();
}