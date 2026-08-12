using Microsoft.AspNetCore.Http;

namespace Appointments.Shared.Common.Errors;

public static class AppointmentResultErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "APPOINTMENT_RESULT_NOT_FOUND",
            $"Appointment result with ID {id} not found.",
            StatusCodes.Status404NotFound);

    public static Error RelatedAppointmentNotFound(Guid appointmentId) =>
        new(
            "APPOINTMENT_RESULT_RELATED_APPOINTMENT_NOT_FOUND",
            $"Appointment with ID {appointmentId} not found.",
            StatusCodes.Status404NotFound);

    public static Error AlreadyExists(Guid appointmentId) =>
        new(
            "APPOINTMENT_RESULT_ALREADY_EXISTS",
            $"Appointment {appointmentId} already has a result.",
            StatusCodes.Status409Conflict);
}