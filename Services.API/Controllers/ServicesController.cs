using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Application.Commands.Services;
using Services.Application.DTO_s.Service;
using Services.Application.Queries.Services;

namespace Services.API.Controllers;

[ApiController]
[Route("api/v1/services")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetServicesQuery { Page = pageNumber, PageSize = pageSize };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetServiceByIdQuery { Id = id };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("consultations")]
    public async Task<IActionResult> GetConsultations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetConsultationsQuery { Page = pageNumber, PageSize = pageSize };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("diagnostics")]
    public async Task<IActionResult> GetDiagnostics([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetDiagnosticsQuery { Page = pageNumber, PageSize = pageSize };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("analyses")]
    public async Task<IActionResult> GetAnalyses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetAnalysesQuery { Page = pageNumber, PageSize = pageSize };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateServiceCommand
        {
            Id = id,
            Name = request.Name,
            Price = request.Price,
            Status = request.Status
        };
    
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteServiceCommand { Id = id };
        
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
}
