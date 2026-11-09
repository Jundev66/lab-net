using LabNet.Projects.Application.DTOs;
using LabNet.Projects.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Projects;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsGetController : ControllerBase
{
    private readonly GetProjectsUseCase _getProjectsUseCase;

    public ProjectsGetController(GetProjectsUseCase getProjectsUseCase)
    {
        _getProjectsUseCase = getProjectsUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Execute(CancellationToken ct)
    {
        var projects = await _getProjectsUseCase.ExecuteAsync(ct);
        return Ok(projects);
    }
}
