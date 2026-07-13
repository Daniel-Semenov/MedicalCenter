
namespace Profiles.DAL.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public bool IsLinkedToAccount { get; set; }
}