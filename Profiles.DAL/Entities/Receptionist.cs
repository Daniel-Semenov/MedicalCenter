namespace Profiles.DAL.Entities;

public class Receptionist
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? OfficeId { get; set; }        
}