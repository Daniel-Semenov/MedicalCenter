namespace Profiles.Domain.Entities;

public class Receptionist
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid? OfficeId { get; set; }        
}