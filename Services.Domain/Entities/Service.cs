using Services.Domain.Enums;

namespace Services.Domain.Entities;

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ServiceCategory Category { get; set; } 
    public ServiceStatus Status { get; set; }    
    public Guid SpecializationId { get; set; }   
    public Specialization Specialization { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}