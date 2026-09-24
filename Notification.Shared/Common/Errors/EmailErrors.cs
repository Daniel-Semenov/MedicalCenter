using Microsoft.AspNetCore.Http;

namespace Notification.Shared.Common.Errors
{
    public static class EmailErrors
    {
        public static Error SendFailed(Guid id) =>
            new(
                "EMAIL_SEND_FAILED",
                $"Email with ID {id} send failed.",
                StatusCodes.Status400BadRequest);
    }
}