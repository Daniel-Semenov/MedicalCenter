using Microsoft.AspNetCore.Http;

namespace Notification.Shared.Common.Errors;


public static class NotificationErrors
{
    public static Error NotFoundByEventId(Guid eventId)
    {
        return new Error(
            "NOTIFICATION_NOT_FOUND",
            $"Notification for event '{eventId}' was not found.",
            StatusCodes.Status404NotFound);
    }

    public static Error DatabaseSaveFailed()
    {
        return new Error(
            "NOTIFICATION_DATABASE_SAVE_FAILED",
            "Failed to save notification.",
            StatusCodes.Status500InternalServerError);
    }
}