using MassTransit;
using MediatR;
using Messaging.Contracts.Events.AppointmentResults;
using Microsoft.Extensions.Logging;
using Notifications.Application.Commands;

namespace Notifications.Infrastructure.Messaging;

public sealed class AppointmentResultConsumer : IConsumer<AppointmentResultCreated>
{
    private readonly ISender _sender;
    private readonly ILogger<AppointmentResultConsumer> _logger;

    public AppointmentResultConsumer(
        ISender sender,
        ILogger<AppointmentResultConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AppointmentResultCreated> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received AppointmentResultCreated event: AppointmentResultId={AppointmentResultId}",
            message.AppointmentResultId);

        var command = new SendAppointmentResultCommand(
            message.Id,
            message.AppointmentResultId,
            message.PatientId,
            message.PatientEmail,
            message.PatientFullName,
            message.ResultDate,
            message.DoctorFullName,
            message.DoctorSpecialization,
            message.ServiceName,
            message.PdfDocumentId);

        var result = await _sender.Send(command, context.CancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError(
                "Failed to send appointment result email: AppointmentResultId={AppointmentResultId}, Error={Error}",
                message.AppointmentResultId,
                result.Error);

            throw new InvalidOperationException(result.Error!.Message);
        }

        _logger.LogInformation(
            "Appointment result email sent: AppointmentResultId={AppointmentResultId}",
            message.AppointmentResultId);
    }
}