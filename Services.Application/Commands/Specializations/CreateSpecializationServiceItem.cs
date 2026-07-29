using Services.Domain.Enums;

namespace Services.Application.Commands.Specializations;

public class CreateSpecializationServiceItem
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public ServiceCategory Category { get; set; }
    public ServiceStatus Status { get; set; } = ServiceStatus.Active;
}