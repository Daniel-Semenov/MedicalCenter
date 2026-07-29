using MediatR;
using Services.Domain.Enums;

namespace Services.Application.Commands.Specializations;

public class CreateSpecializationCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public SpecializationStatus Status { get; set; } = SpecializationStatus.Active;
    public List<CreateSpecializationServiceItem> Services { get; set; } = new();
}