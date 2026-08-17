using MediatR;

namespace Profiles.Application.Notifications.Receptionists;

public record ReceptionistDeletedNotification(
    Guid Id,
    DateTime DeletedAt
) : INotification;