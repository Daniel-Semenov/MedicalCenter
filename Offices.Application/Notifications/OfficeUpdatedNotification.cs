using MediatR;

namespace Offices.Application.Notifications;

public record OfficeUpdatedNotification(
    Guid Id,
    string City,
    string Street,
    int HouseNumber,
    int OfficeNumber,
    string PhoneNumber,
    DateTime UpdatedAt
) : INotification;