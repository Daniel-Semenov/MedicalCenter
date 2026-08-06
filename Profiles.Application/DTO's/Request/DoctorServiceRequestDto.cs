namespace Profiles.Application.DTO_s.Request;

public class DoctorServiceRequestDto
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
}