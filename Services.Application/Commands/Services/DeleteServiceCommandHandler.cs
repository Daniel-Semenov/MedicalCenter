using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;
using Services.Domain.Entities;
using Services.Domain.Exceptions;

namespace Services.Application.Commands.Services;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IPublisher _publisher;

    public DeleteServiceCommandHandler(
        IServiceRepository serviceRepository,
        IPublisher publisher)
    {
        _serviceRepository = serviceRepository;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(nameof(Service), request.Id);
        }

        await _serviceRepository.DeleteAsync(service, cancellationToken);

        await _publisher.Publish(new ServiceDeletedNotification(service.Id), cancellationToken);

        await _serviceRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}