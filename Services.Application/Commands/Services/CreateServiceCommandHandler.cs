using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPublisher _publisher;

    public CreateServiceCommandHandler(
        IServiceRepository serviceRepository,
        ISpecializationRepository specializationRepository,
        IPublisher publisher)
    {
        _serviceRepository = serviceRepository;
        _specializationRepository = specializationRepository;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);

        if (specialization is null)
        {
            throw new NotFoundException(nameof(Specialization), request.SpecializationId);
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

        await _publisher.Publish(new ServiceCreatedNotification(service), cancellationToken);

        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}