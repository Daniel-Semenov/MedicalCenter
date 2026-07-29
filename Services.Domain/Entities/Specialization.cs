using Services.Domain.Enums;

namespace Services.Domain.Entities;

public class Specialization
{
    public Guid Id { get; set; }
    public string Name { get; set; }              
    public SpecializationStatus Status { get; set; }
    public ICollection<Service> Services { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
