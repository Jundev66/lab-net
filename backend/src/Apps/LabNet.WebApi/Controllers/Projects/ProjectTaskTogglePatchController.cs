using LabNet.Projects.Application.DTOs;
using LabNet.Projects.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Projects;

[ApiController]
[Route("api/projects/{id:guid}/tasks/{taskId:guid}/toggle")]
[Authorize]
public class ProjectTaskTogglePatchController : ControllerBase
{
    private readonly ToggleTaskUseCase _toggleTaskUseCase;

    public ProjectTaskTogglePatchController(ToggleTaskUseCase toggleTaskUseCase)
    {
        _toggleTaskUseCase = toggleTaskUseCase;
    }

    [HttpPatch]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Execute([FromRoute] Guid id, [FromRoute] Guid taskId, CancellationToken ct)
    {
        var task = await _toggleTaskUseCase.ExecuteAsync(id, taskId, ct);
        return Ok(task);
    }
}
