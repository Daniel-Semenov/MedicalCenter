using Microsoft.AspNetCore.Mvc;
using Profiles.Application.DTO_s.Request;
using Profiles.Application.Interfaces.Services;
using Profiles.Shared.Common.Constants;

namespace Profiles.API.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> GetByName(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByNameAsync(searchTerm, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
    
    [HttpGet("account/{accountId:guid}")]
    public async Task<IActionResult> GetByAccountId(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.GetByAccountIdAsync(accountId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = PaginationConstants.DefaultPage,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await _patientService.GetPagedAsync(page, pageSize, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.CreateAsync(request, cancellationToken);

        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePatientRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _patientService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}