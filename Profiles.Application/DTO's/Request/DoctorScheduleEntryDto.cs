namespace Profiles.Application.DTO_s.Request;

public class DoctorScheduleEntryDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}