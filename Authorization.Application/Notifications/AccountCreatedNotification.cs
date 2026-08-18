using MediatR;

namespace Authorization.Application.Notifications;

public record AccountCreatedNotification(
    Guid Id,
    string Role,
    string FirstName,
    string LastName,
    string? MiddleName,
    bool IsActive,
    DateTime CreatedAt
) : INotification;