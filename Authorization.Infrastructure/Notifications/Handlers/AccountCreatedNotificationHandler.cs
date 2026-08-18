using Authorization.Application.Interfaces;
using Authorization.Application.Notifications;
using Authorization.Infrastructure.Messaging.Contracts.Events.Accounts;
using MediatR;

namespace Authorization.Infrastructure.Notifications.Handlers;

public class AccountCreatedNotificationHandler : INotificationHandler<AccountCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public AccountCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(AccountCreatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new AccountCreated
        {
            Id = notification.Id,
            Role = notification.Role,
            FirstName = notification.FirstName,
            LastName = notification.LastName,
            MiddleName = notification.MiddleName,
            IsActive = notification.IsActive,
            CreatedAt = notification.CreatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}