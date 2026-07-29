using MediatR;

namespace Services.Application.Commands.Specializations;

public class DeleteSpecializationCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}