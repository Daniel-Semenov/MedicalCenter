using MediatR;

namespace Offices.Application.Notifications;

public record OfficeStatusChangedNotification(
    Guid Id,
    bool IsActive,
    DateTime ChangedAt
) : INotification;