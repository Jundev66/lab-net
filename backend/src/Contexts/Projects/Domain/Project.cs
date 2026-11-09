using LabNet.Shared.Domain;
using LabNet.Shared.Domain.Exceptions;

namespace LabNet.Projects.Domain;

public enum ProjectStatus
{
    Active = 1,
    Archived = 2
}

public class Project : Entity, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ProjectStatus Status { get; private set; } = ProjectStatus.Active;

    private readonly List<TaskItem> _tasks = new();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    private Project() { }

    public Project(Guid tenantId, string name, string description = "")
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID cannot be empty.");

        TenantId = tenantId;
        SetName(name);
        SetDescription(description);
        Status = ProjectStatus.Active;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name cannot be empty.");
        Name = name.Trim();
        MarkUpdated();
    }

    public void SetDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
        MarkUpdated();
    }

    public void Archive()
    {
        Status = ProjectStatus.Archived;
        MarkUpdated();
    }

    public void Activate()
    {
        Status = ProjectStatus.Active;
        MarkUpdated();
    }

    public TaskItem AddTask(string title, string description = "", TaskPriority priority = TaskPriority.Medium)
    {
        var task = new TaskItem(TenantId, Id, title, description, priority);
        _tasks.Add(task);
        MarkUpdated();
        return task;
    }
}
