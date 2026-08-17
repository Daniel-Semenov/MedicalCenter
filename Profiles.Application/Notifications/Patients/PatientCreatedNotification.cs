using MediatR;

namespace Profiles.Application.Notifications.Patients;

public record PatientCreatedNotification(
    Guid Id,
    Guid AccountId,
    bool IsLinkedToAccount,
    DateTime CreatedAt
) : INotification;