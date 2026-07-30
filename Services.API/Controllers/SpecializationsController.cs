using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Application.Commands.Specializations;
using Services.Application.Queries.Specializations;

namespace Services.API.Controllers;

[ApiController]
[Route("api/v1/specializations")]
public class SpecializationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpecializationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetSpecializationsQuery { PageNumber = pageNumber, PageSize = pageSize };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSpecializationByIdQuery { Id = id };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSpecializationCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpecializationCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSpecializationCommand { Id = id };
        
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
}
