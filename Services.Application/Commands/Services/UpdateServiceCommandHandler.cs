using MediatR;
using MassTransit;
using Messaging.Contracts.Events.Services; 
using Messaging.Contracts.Events.Enums;   
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IPublishEndpoint publishEndpoint)
    {
        _serviceRepository = serviceRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }
        
        var exists = await _serviceRepository.ExistsByNameAndSpecializationAsync(request.Name, request.SpecializationId, cancellationToken);
    
        if (exists)
        {
            throw new ExistsByNameAndSpecializationException(request.Name, request.SpecializationId);
        }

        service.Name = request.Name;
        service.Price = request.Price;
        service.Status = request.Status;
        service.UpdatedAt = DateTime.UtcNow;

        var integrationEvent = new ServiceUpdated
        {
            Id = service.Id,
            Name = service.Name,
            SpecializationId = service.SpecializationId,
            Status = (ServiceStatusSnapshot)service.Status, 
            UpdatedAt = service.UpdatedAt
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        await _serviceRepository.UpdateAsync(service, cancellationToken);
        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}