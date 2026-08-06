using Microsoft.AspNetCore.Mvc;
using Profiles.Application.DTO_s.Request;
using Profiles.Application.Interfaces.Services;
using Profiles.Shared.Common.Constants;

namespace Profiles.API.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("account/{accountId:guid}")]
    public async Task<IActionResult> GetByAccountId(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.GetByAccountIdAsync(accountId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = PaginationConstants.DefaultPage,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await _doctorService.GetPagedAsync(page, pageSize, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDoctorRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _doctorService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}