using Appointments.Application.DTO_s.Response;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Appointments.Application.Queries.AppointmentResults;

public class DownloadAppointmentResultQueryHandler : IRequestHandler<DownloadAppointmentResultQuery, Result<FileDownloadDto>>
{
    private readonly IAppointmentResultRepository _appointmentResultRepository;
    private readonly IAppointmentResultDtoBuilder _dtoBuilder;
    private readonly IPdfResultGenerator _pdfResultGenerator;
    private readonly IDocumentsClient _documentsClient;
    private readonly ILogger<DownloadAppointmentResultQueryHandler> _logger;

    public DownloadAppointmentResultQueryHandler(
        IAppointmentResultRepository appointmentResultRepository,
        IAppointmentResultDtoBuilder dtoBuilder,
        IPdfResultGenerator pdfResultGenerator,
        IDocumentsClient documentsClient,
        ILogger<DownloadAppointmentResultQueryHandler> logger)
    {
        _appointmentResultRepository = appointmentResultRepository;
        _dtoBuilder = dtoBuilder;
        _pdfResultGenerator = pdfResultGenerator;
        _documentsClient = documentsClient;
        _logger = logger;
    }

    public async Task<Result<FileDownloadDto>> Handle(DownloadAppointmentResultQuery request, CancellationToken cancellationToken)
    {
        var resultEntityResult = await _appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!resultEntityResult.IsSuccess)
        {
            return Result<FileDownloadDto>.Failure(resultEntityResult.Error);
        }

        var resultEntity = resultEntityResult.Value;
        var fileName = $"appointment-result-{resultEntity.Id}.pdf";

        if (resultEntity.PdfDocumentId.HasValue)
        {
            try
            {
                var content = await _documentsClient.DownloadAsync(resultEntity.PdfDocumentId.Value, cancellationToken);

                return Result<FileDownloadDto>.Success(new FileDownloadDto
                {
                    Content = content,
                    FileName = fileName,
                    ContentType = "application/pdf"
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to fetch stored PDF {DocumentId} for appointment result {AppointmentResultId} from Documents; regenerating on the fly.",
                    resultEntity.PdfDocumentId.Value, resultEntity.Id);
            }
        }

        var dtoResult = await _dtoBuilder.BuildAsync(resultEntity, cancellationToken);

        if (!dtoResult.IsSuccess)
        {
            return Result<FileDownloadDto>.Failure(dtoResult.Error);
        }

        var pdfBytes = _pdfResultGenerator.Generate(dtoResult.Value);

        return Result<FileDownloadDto>.Success(new FileDownloadDto
        {
            Content = pdfBytes,
            FileName = fileName,
            ContentType = "application/pdf"
        });
    }
}