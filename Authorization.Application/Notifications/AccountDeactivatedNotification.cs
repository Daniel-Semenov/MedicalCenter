using MediatR;

namespace Authorization.Application.Notifications;

public record AccountDeactivatedNotification(
    Guid Id,
    DateTime DeactivatedAt
) : INotification;