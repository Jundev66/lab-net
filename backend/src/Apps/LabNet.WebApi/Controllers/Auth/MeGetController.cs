using System.Security.Claims;
using LabNet.Shared.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Auth;

[ApiController]
[Route("api/auth/me")]
[Authorize]
public class MeGetController : ControllerBase
{
    private readonly ICurrentTenantService _currentTenantService;

    public MeGetController(ICurrentTenantService currentTenantService)
    {
        _currentTenantService = currentTenantService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Execute()
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
            TenantId = _currentTenantService.TenantId,
            TenantIdentifier = _currentTenantService.TenantIdentifier
        });
    }
}
