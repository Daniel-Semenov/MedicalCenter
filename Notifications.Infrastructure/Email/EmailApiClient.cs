using Notification.Shared.Common.Results;
using Notifications.Application.DTO_s;
using Notifications.Application.Interfaces;
using Resend;
using EmailMessage = Notifications.Application.DTO_s.EmailMessage;

namespace Notifications.Infrastructure.Email;

public sealed class EmailApiClient : IEmailSender
{
    private readonly IResend _resend;

    public EmailApiClient(IResend resend)
    {
        _resend = resend;
    }

    public async Task<Result<EmailSendResult>> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        var email = new Resend.EmailMessage
        {
            From = "Medical Center <onboarding@resend.dev>",
            Subject = message.Subject,
            HtmlBody = message.HtmlBody
        };

        email.To.Add(message.To);

        if (message.Attachments is { Count: > 0 })
        {
            email.Attachments = new List<Resend.EmailAttachment>();

            foreach (var attachment in message.Attachments)
            {
                email.Attachments.Add(new Resend.EmailAttachment
                {
                    Filename = attachment.FileName,
                    Content  = Convert.ToBase64String(attachment.Content)
                });
            }
        }
        
        var response = await _resend.EmailSendAsync(
            email,
            cancellationToken);

        return Result<EmailSendResult>.Success(
            new EmailSendResult
            {
                ProviderMessageId = response.Content
            });
    }
}