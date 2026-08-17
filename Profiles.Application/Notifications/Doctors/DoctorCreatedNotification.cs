using MediatR;

namespace Profiles.Application.Notifications.Doctors;

public record DoctorCreatedNotification(
    Guid Id,
    Guid AccountId,
    Guid? SpecializationId,
    Guid? OfficeId,
    int CareerStartYear,
    DateTime CreatedAt
) : INotification;