namespace Profiles.Application.DTO_s.Request;

public class UpdateDoctorRequestDto
{
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
    public int CareerStartYear { get; set; }
    public string? PhotoUrl { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? OfficeName { get; set; }
    public string? OfficeAddress { get; set; }
    public string? SpecializationName { get; set; }
    public List<DoctorServiceRequestDto> Services { get; set; } = new();
}