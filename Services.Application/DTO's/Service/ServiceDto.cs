using Services.Domain.Enums;

namespace Services.Application.DTO_s.Service;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public ServiceCategory Category { get; set; }
    public ServiceStatus Status { get; set; }
    public Guid SpecializationId { get; set; }
}