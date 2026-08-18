using MediatR;

namespace Profiles.Application.Notifications.Patients;

public record PatientCreatedNotification(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime? DateOfBirth
) : INotification;