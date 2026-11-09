using FluentAssertions;
using LabNet.Domain.Common.Exceptions;
using LabNet.Domain.Entities;
using Xunit;

namespace LabNet.UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProjectSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var name = "Alpha Initiative";
        var description = "Strategic project";

        // Act
        var project = new Project(tenantId, name, description);

        // Assert
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
    [InlineData(null)]
    public void Constructor_WithEmptyName_ShouldThrowDomainException(string invalidName)
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var action = () => new Project(tenantId, invalidName);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Project name cannot be empty.");
    }

    [Fact]
    public void Constructor_WithEmptyTenantId_ShouldThrowDomainException()
    {
        // Act
        var action = () => new Project(Guid.Empty, "Valid Name");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Tenant ID cannot be empty.");
    }

    [Fact]
    public void AddTask_ShouldAddTaskToProjectWithMatchingTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var project = new Project(tenantId, "Project One");

        // Act
        var task = project.AddTask("Write Unit Tests", "High code coverage", TaskPriority.High);

        // Assert
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
        // Arrange
        var project = new Project(Guid.NewGuid(), "Archivable Project");

        // Act
        project.Archive();

        // Assert
        project.Status.Should().Be(ProjectStatus.Archived);

        // Act 2
        project.Activate();

        // Assert 2
        project.Status.Should().Be(ProjectStatus.Active);
    }
}
