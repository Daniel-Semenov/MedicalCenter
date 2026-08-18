using MediatR;

namespace Authorization.Application.Notifications;

public record AccountUpdatedNotification(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime UpdatedAt
) : INotification;