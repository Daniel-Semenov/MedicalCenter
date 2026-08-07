using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;
using Services.Messaging.Contracts.Events.Services;

namespace Services.Application.Notifications.Handlers.Services;

public class ServiceDeletedNotificationHandler : INotificationHandler<ServiceDeletedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public ServiceDeletedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ServiceDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishAsync(new ServiceDeleted
        {
            Id = notification.ServiceId,
            DeletedAt = DateTime.UtcNow
        }, cancellationToken);
    }
}