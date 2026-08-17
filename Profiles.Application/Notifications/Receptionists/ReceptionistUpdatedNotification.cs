using MediatR;

namespace Profiles.Application.Notifications.Receptionists;

public record ReceptionistUpdatedNotification(
    Guid Id,
    Guid AccountId,
    Guid? OfficeId,
    DateTime UpdatedAt
) : INotification;