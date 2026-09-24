namespace Messaging.Contracts.Events.Doctors;

public record DoctorScheduleEntry
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
}