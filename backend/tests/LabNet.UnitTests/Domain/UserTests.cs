using FluentAssertions;
using LabNet.Domain.Common.Exceptions;
using LabNet.Domain.Entities;
using Xunit;

namespace LabNet.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var email = "admin@acme.com";
        var passwordHash = "$2a$11$something";
        var fullName = "John Doe";

        // Act
        var user = new User(tenantId, email, passwordHash, fullName, "Admin");

        // Assert
        user.TenantId.Should().Be(tenantId);
        user.Email.Should().Be("admin@acme.com");
        user.FullName.Should().Be("John Doe");
        user.Role.Should().Be("Admin");
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
    {
        // Act
        var action = () => new User(Guid.NewGuid(), invalidEmail, "hash", "John", "Admin");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("A valid email address is required.");
    }

    [Fact]
    public void Constructor_WithInvalidRole_ShouldThrowDomainException()
    {
        // Act
        var action = () => new User(Guid.NewGuid(), "user@acme.com", "hash", "John", "SuperGod");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Invalid user role. Allowed: Admin, Member.");
    }
}
