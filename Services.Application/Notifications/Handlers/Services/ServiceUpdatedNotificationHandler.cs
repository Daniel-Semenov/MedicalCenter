using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;
using Services.Messaging.Contracts.Enums;
using Services.Messaging.Contracts.Events.Services;

namespace Services.Application.Notifications.Handlers.Services;

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
            Price = service.Price,
            Status = (ServiceStatus)service.Status,
            UpdatedAt = service.UpdatedAt!.Value
        }, cancellationToken);
    }
}