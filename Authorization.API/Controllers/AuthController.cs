using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;


    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authService.RegisterAsync(
                request,
                cancellationToken);


        if (result.IsFailure)
        {
            return StatusCode(
                result.Error!.StatusCode,
                result.Error);
        }


        return Ok(result.Value);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authService.LoginAsync(
                request,
                cancellationToken);


        if (result.IsFailure)
        {
            return StatusCode(
                result.Error!.StatusCode,
                result.Error);
        }


        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var result =
            await _authService.LogoutAsync(
                cancellationToken);


        if (result.IsFailure)
        {
            return StatusCode(
                result.Error!.StatusCode,
                result.Error);
        }


        return Ok();
    }
}