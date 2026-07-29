using MediatR;
using Services.Domain.Enums;

namespace Services.Application.Commands.Services;

public class CreateServiceCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public ServiceCategory Category { get; set; }
    public ServiceStatus Status { get; set; } = ServiceStatus.Active;
    public Guid SpecializationId { get; set; }
}