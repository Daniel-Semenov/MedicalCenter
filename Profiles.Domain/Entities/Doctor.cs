namespace Profiles.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
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
    public ICollection<DoctorService> Services { get; set; } = new List<DoctorService>();
}