using MediatR;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Receptionists;
using Profiles.Infrastructure.Messaging.Contracts.Events.Receptionist;

namespace Profiles.Infrastructure.Notifications.Handlers.Receptionists;

public class ReceptionistDeletedNotificationHandler : INotificationHandler<ReceptionistDeletedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public ReceptionistDeletedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ReceptionistDeletedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new ReceptionistDeleted
        {
            Id = notification.Id,
            DeletedAt = notification.DeletedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}