using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Appointments.Application.Commands.AppointmentResults;

public class UpdateAppointmentResultCommandHandler : IRequestHandler<UpdateAppointmentResultCommand, Result>
{
    private readonly IAppointmentResultRepository _appointmentResultRepository;
    private readonly IAppointmentResultDtoBuilder _dtoBuilder;
    private readonly IPdfResultGenerator _pdfResultGenerator;
    private readonly IDocumentsClient _documentsClient;
    private readonly ILogger<UpdateAppointmentResultCommandHandler> _logger;

    public UpdateAppointmentResultCommandHandler(
        IAppointmentResultRepository appointmentResultRepository,
        IAppointmentResultDtoBuilder dtoBuilder,
        IPdfResultGenerator pdfResultGenerator,
        IDocumentsClient documentsClient,
        ILogger<UpdateAppointmentResultCommandHandler> logger)
    {
        _appointmentResultRepository = appointmentResultRepository;
        _dtoBuilder = dtoBuilder;
        _pdfResultGenerator = pdfResultGenerator;
        _documentsClient = documentsClient;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        var getResult = await _appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result.Failure(getResult.Error);
        }

        var appointmentResult = getResult.Value;
        var previousPdfDocumentId = appointmentResult.PdfDocumentId;

        appointmentResult.Complaints = request.Complaints;
        appointmentResult.Conclusion = request.Conclusion;
        appointmentResult.Recommendations = request.Recommendations;
        appointmentResult.UpdatedAt = DateTime.UtcNow;

        var dtoResult = await _dtoBuilder.BuildAsync(appointmentResult, cancellationToken);

        if (!dtoResult.IsSuccess)
        {
            return Result.Failure(dtoResult.Error);
        }

        var pdfBytes = _pdfResultGenerator.Generate(dtoResult.Value);
        var fileName = $"appointment-result-{appointmentResult.Id}.pdf";

        var newPdfDocumentId = await _documentsClient.UploadAppointmentResultPdfAsync(
            appointmentResult.Id, pdfBytes, fileName, cancellationToken);

        appointmentResult.PdfDocumentId = newPdfDocumentId;

        var updateResult = await _appointmentResultRepository.UpdateAsync(appointmentResult, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            await TryDeleteOrphanedPdfAsync(newPdfDocumentId, appointmentResult.Id);
            return Result.Failure(updateResult.Error);
        }

        Result saveResult;

        try
        {
            saveResult = await _appointmentResultRepository.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await TryDeleteOrphanedPdfAsync(newPdfDocumentId, appointmentResult.Id);
            throw;
        }

        if (!saveResult.IsSuccess)
        {
            await TryDeleteOrphanedPdfAsync(newPdfDocumentId, appointmentResult.Id);
            return saveResult;
        }

        if (previousPdfDocumentId.HasValue)
        {
            try
            {
                await _documentsClient.DeleteAsync(previousPdfDocumentId.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to delete previous PDF document {DocumentId} for appointment result {AppointmentResultId}. It is now orphaned in Documents.",
                    previousPdfDocumentId.Value, appointmentResult.Id);
            }
        }

        return Result.Success();
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