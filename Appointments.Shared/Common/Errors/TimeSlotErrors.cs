using Microsoft.AspNetCore.Http;

namespace Appointments.Shared.Common.Errors;

public static class TimeSlotErrors
{
    public static Error NotFound(Guid id) =>
        new("TIME_SLOT_NOT_FOUND", $"Time slot with ID {id} not found.", StatusCodes.Status404NotFound);

    public static Error NotAvailable(Guid doctorId, DateTime startTime) =>
        new(
            "TIME_SLOT_NOT_AVAILABLE",
            $"Time slot for doctor {doctorId} at {startTime:O} is not available.",
            StatusCodes.Status409Conflict);

    public static Error DoctorMismatch(Guid timeSlotId, Guid expectedDoctorId) =>
        new(
            "TIME_SLOT_DOCTOR_MISMATCH",
            $"Time slot {timeSlotId} does not belong to doctor {expectedDoctorId}.",
            StatusCodes.Status409Conflict);
}