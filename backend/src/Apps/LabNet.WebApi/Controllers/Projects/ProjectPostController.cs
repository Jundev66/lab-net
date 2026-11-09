using LabNet.Projects.Application.DTOs;
using LabNet.Projects.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Projects;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectPostController : ControllerBase
{
    private readonly CreateProjectUseCase _createProjectUseCase;

    public ProjectPostController(CreateProjectUseCase createProjectUseCase)
    {
        _createProjectUseCase = createProjectUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Execute([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var project = await _createProjectUseCase.ExecuteAsync(request, ct);
        return Created($"/api/projects/{project.Id}", project);
    }
}
