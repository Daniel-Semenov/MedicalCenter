using MediatR;
using Services.Domain.Enums;

namespace Services.Application.Commands.Services;

public class UpdateServiceCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public ServiceStatus Status { get; set; }
}