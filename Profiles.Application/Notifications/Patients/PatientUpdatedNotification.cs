using MediatR;

namespace Profiles.Application.Notifications.Patients;

public record PatientUpdatedNotification(
    Guid Id,
    Guid AccountId,
    bool IsLinkedToAccount,
    DateTime UpdatedAt
) : INotification;