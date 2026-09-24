using MediatR;
using Notification.Shared.Common.Results;

namespace Notifications.Application.Commands;

public record SendAppointmentResultCommand(
    Guid EventId,
    Guid AppointmentResultId,
    Guid PatientId,
    string PatientEmail,
    string PatientFullName,
    DateTime ResultDate,
    string DoctorFullName,
    string DoctorSpecialization,
    string ServiceName,
    Guid PdfDocumentId
) : IRequest<Result>;