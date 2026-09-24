using Notification.Shared.Common.Results;
using Notifications.Application.DTO_s;

namespace Notifications.Application.Interfaces;

public interface IEmailSender
{
    Task<Result<EmailSendResult>> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken);
}