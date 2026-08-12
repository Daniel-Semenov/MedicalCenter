using Appointments.Application.DTO_s.Response;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.AppointmentResults;

public class DownloadAppointmentResultQueryHandler : IRequestHandler<DownloadAppointmentResultQuery, Result<FileDownloadDto>>
{
    private readonly IMediator _mediator;
    private readonly IPdfResultGenerator _pdfResultGenerator;

    public DownloadAppointmentResultQueryHandler(IMediator mediator, IPdfResultGenerator pdfResultGenerator)
    {
        _mediator = mediator;
        _pdfResultGenerator = pdfResultGenerator;
    }

    public async Task<Result<FileDownloadDto>> Handle(DownloadAppointmentResultQuery request, CancellationToken cancellationToken)
    {
        var resultDtoResult = await _mediator.Send(new GetAppointmentResultByIdQuery { Id = request.Id }, cancellationToken);

        if (!resultDtoResult.IsSuccess)
        {
            return Result<FileDownloadDto>.Failure(resultDtoResult.Error);
        }

        var pdfBytes = _pdfResultGenerator.Generate(resultDtoResult.Value);

        var file = new FileDownloadDto
        {
            Content = pdfBytes,
            FileName = $"appointment-result-{request.Id}.pdf",
            ContentType = "application/pdf"
        };

        return Result<FileDownloadDto>.Success(file);
    }
}