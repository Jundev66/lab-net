using LabNet.Application.Projects.DTOs;
using LabNet.Domain.Common.Exceptions;
using LabNet.Domain.Entities;
using LabNet.Domain.Interfaces;

namespace LabNet.Application.Projects.UseCases;

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
