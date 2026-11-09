using LabNet.Projects.Application.DTOs;
using LabNet.Projects.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers.Projects;

[ApiController]
[Route("api/projects/{id:guid}/tasks")]
[Authorize]
public class ProjectTaskPostController : ControllerBase
{
    private readonly AddTaskUseCase _addTaskUseCase;

    public ProjectTaskPostController(AddTaskUseCase addTaskUseCase)
    {
        _addTaskUseCase = addTaskUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Execute([FromRoute] Guid id, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var task = await _addTaskUseCase.ExecuteAsync(id, request, ct);
        return Created($"/api/projects/{id}/tasks/{task.Id}", task);
    }
}
