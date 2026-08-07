using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Specializations;

public class CreateSpecializationCommandHandler : IRequestHandler<CreateSpecializationCommand, Guid>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublisher _publisher;

    public CreateSpecializationCommandHandler(
        ISpecializationRepository specializationRepository,
        IPublisher publisher)
    {
        _specializationRepository = specializationRepository;
        _publisher = publisher;
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

        await _publisher.Publish(new SpecializationCreatedNotification(specialization), cancellationToken);

        await _specializationRepository.SaveChangesAsync(cancellationToken);

        return specialization.Id;
    }
}