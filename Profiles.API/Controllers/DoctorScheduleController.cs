using Microsoft.AspNetCore.Mvc;
using Profiles.Application.DTO_s.Request;
using Profiles.Application.Interfaces.Services;

namespace Profiles.API.Controllers;

[ApiController]
[Route("api/doctors/{doctorId:guid}/schedule")]
public class DoctorScheduleController : ControllerBase
{
    private readonly IDoctorScheduleService _doctorScheduleService;

    public DoctorScheduleController(IDoctorScheduleService doctorScheduleService)
    {
        _doctorScheduleService = doctorScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid doctorId, CancellationToken cancellationToken)
    {
        var result = await _doctorScheduleService.GetByDoctorIdAsync(doctorId, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        Guid doctorId, [FromBody] UpdateDoctorScheduleRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _doctorScheduleService.UpdateAsync(doctorId, request, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}