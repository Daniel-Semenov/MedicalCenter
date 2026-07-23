using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Authorization.Domain.Constants;
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
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse( User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;

        var result = await _authService.LogoutAsync(
            userId,
            jti,
            request,
            cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(result.Error!.StatusCode, result.Error);
        }

        return Ok();
    }
    
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("accounts/{accountId:guid}/roles")]
    public async Task<IActionResult> AddRole(
        Guid accountId,
        AddRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.AddRoleAsync(
            accountId,
            request.Role,
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