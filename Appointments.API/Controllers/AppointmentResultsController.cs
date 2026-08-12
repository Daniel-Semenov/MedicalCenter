using MediatR;
using Microsoft.AspNetCore.Mvc;
using Appointments.API.Extensions;
using Appointments.Application.Commands.AppointmentResults;
using Appointments.Application.DTO_s.Request.Appointment;
using Appointments.Application.DTO_s.Request.AppointmentResult;
using Appointments.Application.Queries.AppointmentResults;

namespace Appointments.API.Controllers;

[ApiController]
[Route("api/appointment-results")]
public class AppointmentResultsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentResultsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppointmentResultByIdQuery { Id = id }, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadAppointmentResultQuery { Id = id }, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToActionResult();
        }

        return File(result.Value!.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentResultRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateAppointmentResultCommand
        {
            AppointmentId = request.AppointmentId,
            Complaints = request.Complaints,
            Conclusion = request.Conclusion,
            Recommendations = request.Recommendations
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentResultRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateAppointmentResultCommand
        {
            Id = id,
            Complaints = request.Complaints,
            Conclusion = request.Conclusion,
            Recommendations = request.Recommendations
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}