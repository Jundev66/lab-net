using LabNet.Shared.Domain;
using LabNet.Shared.Domain.Exceptions;

namespace LabNet.Projects.Domain;

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public class TaskItem : Entity, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
    public bool IsCompleted { get; private set; } = false;

    private TaskItem() { }

    public TaskItem(Guid tenantId, Guid projectId, string title, string description = "", TaskPriority priority = TaskPriority.Medium)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID cannot be empty.");
        if (projectId == Guid.Empty)
            throw new DomainException("Project ID cannot be empty.");

        TenantId = tenantId;
        ProjectId = projectId;
        SetTitle(title);
        SetDescription(description);
        Priority = priority;
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title cannot be empty.");
        Title = title.Trim();
        MarkUpdated();
    }

    public void SetDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
        MarkUpdated();
    }

    public void SetPriority(TaskPriority priority)
    {
        Priority = priority;
        MarkUpdated();
    }

    public void Complete()
    {
        IsCompleted = true;
        MarkUpdated();
    }

    public void Reopen()
    {
        IsCompleted = false;
        MarkUpdated();
    }
}
