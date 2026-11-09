using LabNet.Projects.Application.DTOs;
using LabNet.Projects.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Projects;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectGetByIdController : ControllerBase
{
    private readonly GetProjectByIdUseCase _getProjectByIdUseCase;

    public ProjectGetByIdController(GetProjectByIdUseCase getProjectByIdUseCase)
    {
        _getProjectByIdUseCase = getProjectByIdUseCase;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Execute([FromRoute] Guid id, CancellationToken ct)
    {
        var project = await _getProjectByIdUseCase.ExecuteAsync(id, ct);
        return Ok(project);
    }
}
