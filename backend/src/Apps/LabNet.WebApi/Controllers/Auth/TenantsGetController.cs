using LabNet.Auth.Application.DTOs;
using LabNet.Auth.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Auth;

[ApiController]
[Route("api/auth/tenants")]
public class TenantsGetController : ControllerBase
{
    private readonly GetTenantsUseCase _getTenantsUseCase;

    public TenantsGetController(GetTenantsUseCase getTenantsUseCase)
    {
        _getTenantsUseCase = getTenantsUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TenantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Execute(CancellationToken ct)
    {
        var response = await _getTenantsUseCase.ExecuteAsync(ct);
        return Ok(response);
    }
}
