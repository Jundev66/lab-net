using FluentAssertions;
using LabNet.Projects.Domain;
using LabNet.Shared.Domain.Exceptions;
using Xunit;

namespace LabNet.UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProjectSuccessfully()
    {
        var tenantId = Guid.NewGuid();
        var name = "Alpha Initiative";
        var description = "Strategic project";

        var project = new Project(tenantId, name, description);

        project.Id.Should().NotBeEmpty();
        project.TenantId.Should().Be(tenantId);
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.Status.Should().Be(ProjectStatus.Active);
        project.Tasks.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_ShouldThrowDomainException(string invalidName)
    {
        var tenantId = Guid.NewGuid();
        var action = () => new Project(tenantId, invalidName);

        action.Should().Throw<DomainException>()
            .WithMessage("Project name cannot be empty.");
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowDomainException()
    {
        var action = () => new Project(Guid.Empty, "Valid Name");

        action.Should().Throw<DomainException>()
            .WithMessage("Tenant ID cannot be empty.");
    }

    [Fact]
    public void AddTask_ShouldAddTaskToProjectWithMatchingTenantId()
    {
        var tenantId = Guid.NewGuid();
        var project = new Project(tenantId, "Project One");

        var task = project.AddTask("Write Unit Tests", "High code coverage", TaskPriority.High);

        project.Tasks.Should().HaveCount(1);
        task.TenantId.Should().Be(tenantId);
        task.ProjectId.Should().Be(project.Id);
        task.Title.Should().Be("Write Unit Tests");
        task.Priority.Should().Be(TaskPriority.High);
        task.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void ArchiveAndActivate_ShouldChangeProjectStatus()
    {
        var project = new Project(Guid.NewGuid(), "Archivable Project");

        project.Archive();
        project.Status.Should().Be(ProjectStatus.Archived);

        project.Activate();
        project.Status.Should().Be(ProjectStatus.Active);
    }
}
