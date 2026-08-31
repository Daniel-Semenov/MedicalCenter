namespace Appointments.Application.Options;

public class TimeSlotGenerationOptions
{
    public const string SectionName = "TimeSlotGeneration";

    public string CronSchedule { get; set; } = "1 * * * * ?";
    public int WindowDays { get; set; } = 14;
}