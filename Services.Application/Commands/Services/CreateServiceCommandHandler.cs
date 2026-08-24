using MediatR;
using MassTransit;
using Messaging.Contracts.Events.Services;
using Messaging.Contracts.Events.Enums;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublishEndpoint _publishEndpoint; 

    public CreateServiceCommandHandler(
        IServiceRepository serviceRepository,
        ISpecializationRepository specializationRepository,
        IPublishEndpoint publishEndpoint) 
    {
        _serviceRepository = serviceRepository;
        _specializationRepository = specializationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.SpecializationId);
        }
        
        var exists = await _serviceRepository.ExistsByNameAndSpecializationAsync(request.Name, request.SpecializationId, cancellationToken);
    
        if (exists)
        {
            throw new ExistsByNameAndSpecializationException(request.Name, request.SpecializationId);
        }

        var service = new Service
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            Status = request.Status,
            SpecializationId = request.SpecializationId,
            CreatedAt = DateTime.UtcNow
        };

        await _serviceRepository.AddAsync(service, cancellationToken);

        var integrationEvent = new ServiceCreated
        {
            Id = service.Id,
            Name = service.Name,
            SpecializationId = service.SpecializationId,
            Status = (ServiceStatusSnapshot)service.Status,
            CreatedAt = service.CreatedAt
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}