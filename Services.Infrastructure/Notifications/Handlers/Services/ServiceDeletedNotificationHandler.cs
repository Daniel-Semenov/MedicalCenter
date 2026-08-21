using MediatR;
using Messaging.Contracts.Events.Services;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;

namespace Services.Infrastructure.Notifications.Handlers.Services;

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