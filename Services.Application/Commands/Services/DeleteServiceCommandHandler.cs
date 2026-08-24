using MediatR;
using MassTransit;
using Messaging.Contracts.Events.Services;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IPublishEndpoint _publishEndpoint; 

    public DeleteServiceCommandHandler(
        IServiceRepository serviceRepository,
        IPublishEndpoint publishEndpoint) 
    {
        _serviceRepository = serviceRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }

        await _serviceRepository.DeleteAsync(service, cancellationToken);

        var integrationEvent = new ServiceDeleted
        {
            Id = service.Id,
            DeletedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}