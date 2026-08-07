using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications;
using Services.Application.Notifications.Services;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IPublisher _publisher;

    public UpdateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IPublisher publisher)
    {
        _serviceRepository = serviceRepository;
        _publisher = publisher;
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

        await _serviceRepository.UpdateAsync(service, cancellationToken);

        await _publisher.Publish(new ServiceUpdatedNotification(service), cancellationToken);

        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}