using MediatR;
using MassTransit;
using Messaging.Contracts.Events.Specializations;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Services;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class CreateSpecializationCommandHandler : IRequestHandler<CreateSpecializationCommand, Guid>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublishEndpoint _publishEndpoint; 

    public CreateSpecializationCommandHandler(
        ISpecializationRepository specializationRepository,
        IPublishEndpoint publishEndpoint) 
    {
        _specializationRepository = specializationRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specializationExists = await _specializationRepository.ExistsByNameAsync(request.Name, cancellationToken);
        
        if (specializationExists)
        {
            throw new SpecializationAlreadyExistsException(request.Name);
        }

        var duplicateServices = request.Services
            .GroupBy(s => s.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateServices.Any())
        {
            throw new DuplicateServiceNamesException(duplicateServices);
        }
        
        var specialization = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            Services = request.Services.Select(s => new Service
            {
                Id = Guid.NewGuid(),
                Name = s.Name,
                Price = s.Price,
                Category = s.Category,
                Status = s.Status,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        await _specializationRepository.AddAsync(specialization, cancellationToken);

        var integrationEvent = new SpecializationCreated
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Status = (SpecializationStatusSnapshot)specialization.Status,
            CreatedAt = specialization.CreatedAt,
            Services = specialization.Services.Select(s => new SpecializationServiceItemSnapshot
            {
                Id = s.Id,
                Name = s.Name,
                Status = (ServiceStatusSnapshot)s.Status,
            }).ToList()
        };

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        await _specializationRepository.SaveChangesAsync(cancellationToken);

        return specialization.Id;
    }
}