using MediatR;
using Microsoft.AspNetCore.Mvc;
using Appointments.API.Extensions;
using Appointments.Application.Commands.Appointments;
using Appointments.Application.DTO_s.Request.Appointment;
using Appointments.Application.Queries.Appointments;
using Appointments.Shared.Common.Constants;

namespace Appointments.API.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery { Id = id }, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(
        Guid patientId,
        [FromQuery] int page = PaginationConstants.DefaultPage,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAppointmentsByPatientQuery { PatientId = patientId, Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetByDoctor(
        Guid doctorId,
        [FromQuery] int page = PaginationConstants.DefaultPage,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAppointmentsByDoctorQuery { DoctorId = doctorId, Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateAppointmentCommand
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            ServiceId = request.ServiceId,
            OfficeId = request.OfficeId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(
        Guid id,
        [FromBody] RescheduleAppointmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RescheduleAppointmentCommand
        {
            Id = id,
            NewAppointmentDate = request.NewAppointmentDate,
            NewStartTime = request.NewStartTime
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelAppointmentCommand { Id = id }, cancellationToken);

        return result.ToActionResult();
    }
}