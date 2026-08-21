using MediatR;

namespace Offices.Application.Notifications;

public record OfficeDeletedNotification(
    Guid Id,
    DateTime DeletedAt
) : INotification;