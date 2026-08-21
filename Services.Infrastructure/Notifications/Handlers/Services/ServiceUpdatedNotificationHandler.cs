using MediatR;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Services;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;

namespace Services.Infrastructure.Notifications.Handlers.Services;

public class ServiceUpdatedNotificationHandler : INotificationHandler<ServiceUpdatedNotification> 
{
    private readonly IEventPublisher _eventPublisher;

    public ServiceUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ServiceUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var service = notification.Service;

        await _eventPublisher.PublishAsync(new ServiceUpdated
        {
            Id = service.Id,
            Name = service.Name,
            SpecializationId = service.SpecializationId,
            Status = (ServiceStatusSnapshot)service.Status,
            UpdatedAt = service.UpdatedAt!.Value
        }, cancellationToken);
    }
}