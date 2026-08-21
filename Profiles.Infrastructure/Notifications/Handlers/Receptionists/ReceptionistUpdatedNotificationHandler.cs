using MediatR;
using Messaging.Contracts.Events.Receptionist;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Receptionists;

namespace Profiles.Infrastructure.Notifications.Handlers.Receptionists;

public class ReceptionistUpdatedNotificationHandler : INotificationHandler<ReceptionistUpdatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public ReceptionistUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ReceptionistUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new ReceptionistUpdated
        {
            Id = notification.Id,
            AccountId = notification.AccountId,
            OfficeId = notification.OfficeId,
            UpdatedAt = notification.UpdatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}