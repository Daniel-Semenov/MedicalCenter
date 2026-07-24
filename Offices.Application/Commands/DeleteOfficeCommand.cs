using MediatR;

namespace Offices.Application.Commands;

public class DeleteOfficeCommand : IRequest
{
    public Guid Id { get; set; }
}