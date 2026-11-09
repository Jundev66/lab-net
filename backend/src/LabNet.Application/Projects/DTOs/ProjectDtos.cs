using LabNet.Domain.Entities;

namespace LabNet.Application.Projects.DTOs;

public record TaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Description,
    TaskPriority Priority,
    bool IsCompleted,
    DateTime CreatedAtUtc
);

public record ProjectResponse(
    Guid Id,
    string Name,
    string Description,
    ProjectStatus Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<TaskResponse> Tasks
);

public record CreateProjectRequest(string Name, string Description);

public record CreateTaskRequest(string Title, string Description, TaskPriority Priority);
