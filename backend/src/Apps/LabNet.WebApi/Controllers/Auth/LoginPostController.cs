using LabNet.Auth.Application.DTOs;
using LabNet.Auth.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Auth;

[ApiController]
[Route("api/auth/login")]
public class LoginPostController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public LoginPostController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Execute([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _loginUseCase.ExecuteAsync(request, ct);
        return Ok(response);
    }
}
