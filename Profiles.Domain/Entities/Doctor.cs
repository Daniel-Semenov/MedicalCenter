namespace Profiles.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }        
    public int CareerStartYear { get; set; }
}