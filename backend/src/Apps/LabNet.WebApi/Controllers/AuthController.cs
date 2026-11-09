using System.Security.Claims;
using LabNet.Auth.Application.DTOs;
using LabNet.Auth.Application.UseCases;
using LabNet.Shared.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly GetTenantsUseCase _getTenantsUseCase;

    public AuthController(LoginUseCase loginUseCase, GetTenantsUseCase getTenantsUseCase)
    {
        _loginUseCase = loginUseCase;
        _getTenantsUseCase = getTenantsUseCase;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _loginUseCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpGet("tenants")]
    [ProducesResponseType(typeof(IReadOnlyList<TenantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenants(CancellationToken ct)
    {
        var response = await _getTenantsUseCase.ExecuteAsync(ct);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser([FromServices] ICurrentTenantService currentTenantService)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var name = User.FindFirst("name")?.Value;

        return Ok(new
        {
            UserId = userId,
            Email = email,
            FullName = name,
            Role = role,
            TenantId = currentTenantService.TenantId,
            TenantIdentifier = currentTenantService.TenantIdentifier
        });
    }
}
