using MediatR;

namespace Profiles.Application.Notifications.Receptionists;

public record ReceptionistCreatedNotification(
    Guid Id,
    Guid AccountId,
    Guid? OfficeId,
    DateTime CreatedAt
) : INotification;