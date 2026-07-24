using MediatR;

namespace Offices.Application.Commands;

public class ToggleOfficeStatusCommand : IRequest
{
    public Guid Id { get; set; }
}