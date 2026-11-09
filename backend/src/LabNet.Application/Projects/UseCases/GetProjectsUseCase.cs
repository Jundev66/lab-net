using LabNet.Application.Projects.DTOs;
using LabNet.Domain.Interfaces;

namespace LabNet.Application.Projects.UseCases;

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
