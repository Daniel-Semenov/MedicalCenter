using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;

namespace Offices.Infrastructure.Notifications.Handlers;

public class OfficeDeletedNotificationHandler : INotificationHandler<OfficeDeletedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public OfficeDeletedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(OfficeDeletedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new OfficeDeleted
        {
            Id = notification.Id,
            DeletedAt = notification.DeletedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}