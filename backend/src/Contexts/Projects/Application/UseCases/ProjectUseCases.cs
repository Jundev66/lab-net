using LabNet.Projects.Application.DTOs;
using LabNet.Projects.Domain;
using LabNet.Shared.Domain.Exceptions;
using LabNet.Shared.Domain.Interfaces;

namespace LabNet.Projects.Application.UseCases;

public class GetProjectsUseCase
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsUseCase(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyList<ProjectResponse>> ExecuteAsync(CancellationToken ct = default)
    {
        var projects = await _projectRepository.GetAllAsync(ct);
        return projects.Select(p => new ProjectResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Status,
            p.CreatedAtUtc,
            p.Tasks.Select(t => new TaskResponse(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Description,
                t.Priority,
                t.IsCompleted,
                t.CreatedAtUtc
            )).ToList()
        )).ToList();
    }
}

public class GetProjectByIdUseCase
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdUseCase(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectResponse> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, ct);
        if (project is null)
            throw new EntityNotFoundException(nameof(Project), id);

        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.Status,
            project.CreatedAtUtc,
            project.Tasks.Select(t => new TaskResponse(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Description,
                t.Priority,
                t.IsCompleted,
                t.CreatedAtUtc
            )).ToList()
        );
    }
}

public class CreateProjectUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantService _currentTenantService;

    public CreateProjectUseCase(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentTenantService currentTenantService)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentTenantService = currentTenantService;
    }

    public async Task<ProjectResponse> ExecuteAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        if (!_currentTenantService.HasTenant)
            throw new DomainException("Tenant context is missing.");

        var project = new Project(_currentTenantService.TenantId!.Value, request.Name, request.Description);
        await _projectRepository.AddAsync(project, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.Status,
            project.CreatedAtUtc,
            Array.Empty<TaskResponse>()
        );
    }
}

public class AddTaskUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddTaskUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskResponse> ExecuteAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct);
        if (project is null)
            throw new EntityNotFoundException(nameof(Project), projectId);

        var task = project.AddTask(request.Title, request.Description, request.Priority);
        await _unitOfWork.SaveChangesAsync(ct);

        return new TaskResponse(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.Priority,
            task.IsCompleted,
            task.CreatedAtUtc
        );
    }
}

public class ToggleTaskUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleTaskUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskResponse> ExecuteAsync(Guid projectId, Guid taskId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct);
        if (project is null)
            throw new EntityNotFoundException(nameof(Project), projectId);

        var task = project.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task is null)
            throw new EntityNotFoundException(nameof(TaskItem), taskId);

        if (task.IsCompleted)
            task.Reopen();
        else
            task.Complete();

        await _unitOfWork.SaveChangesAsync(ct);

        return new TaskResponse(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.Priority,
            task.IsCompleted,
            task.CreatedAtUtc
        );
    }
}
