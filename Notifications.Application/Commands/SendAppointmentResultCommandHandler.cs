using MediatR;
using Notification.Shared.Common.Results;
using Notifications.Application.DTO_s;
using Notifications.Application.Interfaces;
using Notifications.Domain.Enums;

namespace Notifications.Application.Commands;

public sealed class SendAppointmentResultCommandHandler
    : IRequestHandler<SendAppointmentResultCommand, Result>
{
    private readonly IEmailSender _emailSender;
    private readonly IDocumentsClient _documentsClient;
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailBodyBuilder _emailBodyBuilder;

    public SendAppointmentResultCommandHandler(
        IEmailSender emailSender,
        IDocumentsClient documentsClient,
        INotificationRepository notificationRepository,
        IEmailBodyBuilder emailBodyBuilder)
    {
        _emailSender = emailSender;
        _documentsClient = documentsClient;
        _notificationRepository = notificationRepository;
        _emailBodyBuilder = emailBodyBuilder;
    }

    public async Task<Result> Handle(
        SendAppointmentResultCommand request,
        CancellationToken cancellationToken)
    {
        var existingNotification = await _notificationRepository
            .GetByEventIdAsync(request.EventId, cancellationToken);

        if (existingNotification.IsSuccess)
        {
            return Result.Success();
        }

        var notification = new Domain.Entities.Notification(
            request.EventId,
            request.PatientId,
            request.PatientEmail,
            NotificationType.AppointmentResult);

        var addResult = await _notificationRepository.AddAsync(
            notification,
            cancellationToken);

        if (!addResult.IsSuccess)
        {
            return Result.Failure(addResult.Error!);
        }

        var documentResult = await _documentsClient.DownloadAsync(
            request.PdfDocumentId,
            cancellationToken);

        if (!documentResult.IsSuccess)
        {
            notification.MarkAsFailed(documentResult.Error!.Message);

            await _notificationRepository.SaveChangesAsync(
                cancellationToken);

            return Result.Failure(documentResult.Error);
        }

        var document = documentResult.Value;

        var email = new EmailMessage
        {
            To = request.PatientEmail,
            Subject = "Result of an appointment",
            HtmlBody = _emailBodyBuilder.BuildAppointmentResultEmail(request),
            Attachments =
            [
                new DocumentFile
                {
                    FileName = document.FileName,
                    ContentType = document.ContentType,
                    Content = document.Content
                }
            ]
        };

        var sendResult = await _emailSender.SendAsync(
            email,
            cancellationToken);

        if (!sendResult.IsSuccess)
        {
            notification.MarkAsFailed(
                sendResult.Error!.Message);

            await _notificationRepository.SaveChangesAsync(
                cancellationToken);

            return Result.Failure(sendResult.Error);
        }

        notification.MarkAsSent(sendResult.Value.ProviderMessageId);

        return await _notificationRepository.SaveChangesAsync(cancellationToken);
    }
}