namespace Profiles.DAL.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }        
    public int CareerStartYear { get; set; }
}