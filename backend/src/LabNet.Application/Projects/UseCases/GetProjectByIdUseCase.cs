using LabNet.Application.Projects.DTOs;
using LabNet.Domain.Common.Exceptions;
using LabNet.Domain.Entities;
using LabNet.Domain.Interfaces;

namespace LabNet.Application.Projects.UseCases;

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
