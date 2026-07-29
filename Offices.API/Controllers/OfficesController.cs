using MediatR;
using Microsoft.AspNetCore.Mvc;
using Offices.Application.Commands;
using Offices.Application.DTO_s;
using Offices.Application.Queries;

namespace Offices.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfficesController : ControllerBase
{
    private readonly IMediator _mediator;

    public OfficesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllOfficesQuery
        {
            Page = page,
            PageSize = pageSize
        };
    
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOfficeByIdQuery { Id = id }, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfficeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfficeRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new UpdateOfficeCommand
        {
            Id = id,
            City = request.City,
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            OfficeNumber = request.OfficeNumber,
            PhoneNumber = request.PhoneNumber
        };
    
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ToggleOfficeStatusCommand { Id = id }, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteOfficeCommand { Id = id }, cancellationToken);
        
        return NoContent();
    }

    [HttpPut("{id}/photo")]
    public async Task<IActionResult> UpdatePhoto(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();
        
        var command = new UpdateOfficePhotoCommand
        {
            OfficeId = id,
            PhotoStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
}