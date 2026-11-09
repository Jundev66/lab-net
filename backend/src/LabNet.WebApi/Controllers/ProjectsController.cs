using LabNet.Application.Projects.DTOs;
using LabNet.Application.Projects.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabNet.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly GetProjectsUseCase _getProjectsUseCase;
    private readonly GetProjectByIdUseCase _getProjectByIdUseCase;
    private readonly CreateProjectUseCase _createProjectUseCase;
    private readonly AddTaskUseCase _addTaskUseCase;
    private readonly ToggleTaskUseCase _toggleTaskUseCase;

    public ProjectsController(
        GetProjectsUseCase getProjectsUseCase,
        GetProjectByIdUseCase getProjectByIdUseCase,
        CreateProjectUseCase createProjectUseCase,
        AddTaskUseCase addTaskUseCase,
        ToggleTaskUseCase toggleTaskUseCase)
    {
        _getProjectsUseCase = getProjectsUseCase;
        _getProjectByIdUseCase = getProjectByIdUseCase;
        _createProjectUseCase = createProjectUseCase;
        _addTaskUseCase = addTaskUseCase;
        _toggleTaskUseCase = toggleTaskUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var projects = await _getProjectsUseCase.ExecuteAsync(ct);
        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var project = await _getProjectByIdUseCase.ExecuteAsync(id, ct);
        return Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var project = await _createProjectUseCase.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPost("{id:guid}/tasks")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTask([FromRoute] Guid id, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var task = await _addTaskUseCase.ExecuteAsync(id, request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, task);
    }

    [HttpPatch("{id:guid}/tasks/{taskId:guid}/toggle")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleTask([FromRoute] Guid id, [FromRoute] Guid taskId, CancellationToken ct)
    {
        var task = await _toggleTaskUseCase.ExecuteAsync(id, taskId, ct);
        return Ok(task);
    }
}
