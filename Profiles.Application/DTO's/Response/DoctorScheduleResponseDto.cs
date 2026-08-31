using Profiles.Application.DTO_s.Request;

namespace Profiles.Application.DTO_s.Response;

public class DoctorScheduleResponseDto
{
    public Guid DoctorId { get; set; }
    public List<DoctorScheduleEntryDto> Entries { get; set; } = new();
}