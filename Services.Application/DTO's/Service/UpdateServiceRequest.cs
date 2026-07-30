using Services.Domain.Enums;

namespace Services.Application.DTO_s.Service;

public class UpdateServiceRequest
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public ServiceStatus Status { get; set; }
}