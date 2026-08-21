
using MediatR;
using Messaging.Contracts.Events.Receptionist;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Receptionists;

namespace Profiles.Infrastructure.Notifications.Handlers.Receptionists;

public class ReceptionistCreatedNotificationHandler : INotificationHandler<ReceptionistCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public ReceptionistCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ReceptionistCreatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new ReceptionistCreated
        {
            Id = notification.Id,
            AccountId = notification.AccountId,
            OfficeId = notification.OfficeId,
            CreatedAt = notification.CreatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}