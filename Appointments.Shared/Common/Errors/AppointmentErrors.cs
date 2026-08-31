using Microsoft.AspNetCore.Http;

namespace Appointments.Shared.Common.Errors;

public static class AppointmentErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "APPOINTMENT_NOT_FOUND",
            $"Appointment with ID {id} not found.",
            StatusCodes.Status404NotFound);

    public static Error SlotAlreadyTaken(Guid doctorId, DateTime date, DateTime startTime) =>
        new(
            "APPOINTMENT_SLOT_ALREADY_TAKEN",
            $"Doctor {doctorId} already has an appointment on {date} at {startTime}.",
            StatusCodes.Status409Conflict);

    public static Error InvalidStatusTransition(Guid id, string currentStatus) =>
        new(
            "APPOINTMENT_INVALID_STATUS_TRANSITION",
            $"Appointment {id} cannot be modified because its current status is {currentStatus}.",
            StatusCodes.Status409Conflict);
    
    public static Error ConcurrencyConflict() =>
        new(
            "APPOINTMENT_CONCURRENCY_CONFLICT",
            "The selected time slot was booked by someone else. Please choose another slot.",
            StatusCodes.Status409Conflict);
}