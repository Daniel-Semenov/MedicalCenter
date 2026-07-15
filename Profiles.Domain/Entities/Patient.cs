
namespace Profiles.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public bool IsLinkedToAccount { get; set; }
}