using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MassTransit;
using MediatR;
using Messaging.Contracts.Events.AppointmentResults;
using Microsoft.Extensions.Logging;

namespace Appointments.Application.Commands.AppointmentResults;

public class CreateAppointmentResultCommandHandler : IRequestHandler<CreateAppointmentResultCommand, Result<Guid>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentResultRepository _appointmentResultRepository;
    private readonly IAppointmentResultDtoBuilder _dtoBuilder;
    private readonly IPdfResultGenerator _pdfResultGenerator;
    private readonly IDocumentsClient _documentsClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreateAppointmentResultCommandHandler> _logger;

    public CreateAppointmentResultCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentResultRepository appointmentResultRepository,
        IAppointmentResultDtoBuilder dtoBuilder,
        IPdfResultGenerator pdfResultGenerator,
        IDocumentsClient documentsClient,
        ILogger<CreateAppointmentResultCommandHandler> logger, 
        IPublishEndpoint publishEndpoint)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentResultRepository = appointmentResultRepository;
        _dtoBuilder = dtoBuilder;
        _pdfResultGenerator = pdfResultGenerator;
        _documentsClient = documentsClient;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        var appointmentResult = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (!appointmentResult.IsSuccess)
        {
            return Result<Guid>.Failure(AppointmentResultErrors.RelatedAppointmentNotFound(request.AppointmentId));
        }

        var existingResult = await _appointmentResultRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);

        if (existingResult.IsSuccess)
        {
            return Result<Guid>.Failure(AppointmentResultErrors.AlreadyExists(request.AppointmentId));
        }

        var appointmentResultEntity = new AppointmentResult
        {
            Id = Guid.NewGuid(),
            AppointmentId = request.AppointmentId,
            Complaints = request.Complaints,
            Conclusion = request.Conclusion,
            Recommendations = request.Recommendations,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await _appointmentResultRepository.AddAsync(appointmentResultEntity, cancellationToken);

        if (!addResult.IsSuccess)
        {
            return Result<Guid>.Failure(addResult.Error);
        }

        var appointment = appointmentResult.Value;
        appointment.Status = AppointmentStatus.Completed;
        appointment.UpdatedAt = DateTime.UtcNow;

        var updateAppointmentResult = await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        if (!updateAppointmentResult.IsSuccess)
        {
            return Result<Guid>.Failure(updateAppointmentResult.Error);
        }

        var dtoResult = await _dtoBuilder.BuildAsync(appointmentResultEntity, cancellationToken);

        if (!dtoResult.IsSuccess)
        {
            return Result<Guid>.Failure(dtoResult.Error);
        }

        var pdfBytes = _pdfResultGenerator.Generate(dtoResult.Value);
        var fileName = $"appointment-result-{appointmentResultEntity.Id}.pdf";

        var pdfDocumentId = await _documentsClient.UploadAppointmentResultPdfAsync(
            appointmentResultEntity.Id, pdfBytes, fileName, cancellationToken);

        try
        {
            appointmentResultEntity.PdfDocumentId = pdfDocumentId;
            
            var integrationEvent = new AppointmentResultCreated
            {
                Id = Guid.NewGuid(),
                AppointmentResultId = appointmentResultEntity.Id,
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                PatientEmail = dtoResult.Value.PatientEmail, 
                PatientFullName = dtoResult.Value.PatientFullName,
                ResultDate = dtoResult.Value.ResultDate,
                DoctorFullName = dtoResult.Value.DoctorFullName,
                DoctorSpecialization = dtoResult.Value.DoctorSpecialization,
                ServiceName = dtoResult.Value.ServiceName,
                PdfDocumentId = pdfDocumentId,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            
            var saveResult = await _appointmentResultRepository.SaveChangesAsync(cancellationToken);

            if (!saveResult.IsSuccess)
            {
                await TryDeleteOrphanedPdfAsync(pdfDocumentId, appointmentResultEntity.Id);
                return Result<Guid>.Failure(saveResult.Error);
            }
            
            return Result<Guid>.Success(appointmentResultEntity.Id);
        }
        catch
        {
            await TryDeleteOrphanedPdfAsync(pdfDocumentId, appointmentResultEntity.Id);
            throw;
        }
    }

    private async Task TryDeleteOrphanedPdfAsync(Guid pdfDocumentId, Guid appointmentResultId)
    {
        try
        {
            await _documentsClient.DeleteAsync(pdfDocumentId, CancellationToken.None);
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx,
                "Failed to clean up orphaned PDF document {DocumentId} for appointment result {AppointmentResultId} after a failed save.",
                pdfDocumentId, appointmentResultId);
        }
    }
}