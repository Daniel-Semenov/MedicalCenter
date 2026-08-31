namespace Profiles.Application.DTO_s.Request;

public class UpdateDoctorScheduleRequestDto
{
    public List<DoctorScheduleEntryDto> Entries { get; set; } = new();
}