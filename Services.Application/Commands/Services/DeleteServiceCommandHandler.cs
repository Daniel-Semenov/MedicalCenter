using MediatR;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;

    public DeleteServiceCommandHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Unit> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }

        await _serviceRepository.DeleteAsync(service, cancellationToken);

        return Unit.Value;
    }
}