using Authorization.Application.Interfaces;
using Authorization.Application.Notifications;
using MediatR;
using Messaging.Contracts.Events.Accounts;

namespace Authorization.Infrastructure.Notifications.Handlers;

public class AccountDeactivatedNotificationHandler : INotificationHandler<AccountDeactivatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public AccountDeactivatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(AccountDeactivatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new AccountDeactivated
        {
            Id = notification.Id,
            DeactivatedAt = notification.DeactivatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}