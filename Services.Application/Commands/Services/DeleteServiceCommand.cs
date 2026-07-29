using MediatR;

namespace Services.Application.Commands.Services;

public class DeleteServiceCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}