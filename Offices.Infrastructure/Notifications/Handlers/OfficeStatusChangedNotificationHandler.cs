using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;

namespace Offices.Infrastructure.Notifications.Handlers;

public class OfficeStatusChangedNotificationHandler : INotificationHandler<OfficeStatusChangedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public OfficeStatusChangedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(OfficeStatusChangedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new OfficeStatusChanged
        {
            Id = notification.Id,
            IsActive = notification.IsActive,
            ChangedAt = notification.ChangedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}