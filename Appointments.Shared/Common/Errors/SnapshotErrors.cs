using Microsoft.AspNetCore.Http;

namespace Appointments.Shared.Common.Errors;

public static class SnapshotErrors
{
    public static Error NotFound(string snapshotType, Guid id) =>
        new(
            "SNAPSHOT_NOT_FOUND",
            $"{snapshotType} snapshot with ID {id} not found.",
            StatusCodes.Status404NotFound);
}