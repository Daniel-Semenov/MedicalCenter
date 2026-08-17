using MediatR;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Patients;
using Profiles.Infrastructure.Messaging.Contracts.Events.Patients;

namespace Profiles.Infrastructure.Notifications.Handlers.Patients;

public class PatientCreatedNotificationHandler : INotificationHandler<PatientCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public PatientCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(PatientCreatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new PatientCreated
        {
            Id = notification.Id,
            AccountId = notification.AccountId,
            IsLinkedToAccount = notification.IsLinkedToAccount,
            CreatedAt = notification.CreatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}