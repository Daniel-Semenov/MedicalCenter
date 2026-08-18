using Authorization.Application.Interfaces;
using Authorization.Application.Notifications;
using Authorization.Infrastructure.Messaging.Contracts.Events.Accounts;
using MediatR;

namespace Authorization.Infrastructure.Notifications.Handlers;

public class AccountUpdatedNotificationHandler : INotificationHandler<AccountUpdatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public AccountUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(AccountUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new AccountUpdated
        {
            Id = notification.Id,
            FirstName = notification.FirstName,
            LastName = notification.LastName,
            MiddleName = notification.MiddleName,
            UpdatedAt = notification.UpdatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}