using MediatR;
using Services.Domain.Enums;

namespace Services.Application.Commands.Specializations;

public class UpdateSpecializationCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public SpecializationStatus Status { get; set; }
}