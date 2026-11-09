using FluentAssertions;
using LabNet.Auth.Application.DTOs;
using LabNet.Auth.Application.Interfaces;
using LabNet.Auth.Application.UseCases;
using LabNet.Auth.Domain;
using LabNet.Shared.Domain.Exceptions;
using LabNet.Shared.Domain.Interfaces;
using Moq;
using Xunit;

namespace LabNet.UnitTests.Application;

public class LoginUseCaseTests
{
    private readonly Mock<ITenantRepository> _tenantRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock = new();
    private readonly Mock<ICurrentTenantService> _tenantServiceMock = new();

    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _useCase = new LoginUseCase(
            _tenantRepoMock.Object,
            _userRepoMock.Object,
            _passwordHasherMock.Object,
            _jwtGeneratorMock.Object,
            _tenantServiceMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        var tenant = new Tenant("Acme Corp", "acme");
        var user = new User(tenant.Id, "admin@acme.com", "hashed_password", "Admin User", "Admin");

        _tenantRepoMock.Setup(r => r.GetByIdentifierAsync("acme", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _userRepoMock.Setup(r => r.GetByEmailAsync("admin@acme.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(h => h.Verify("CorrectPassword123!", "hashed_password"))
            .Returns(true);

        _jwtGeneratorMock.Setup(j => j.GenerateToken(user, tenant))
            .Returns("fake_jwt_token");

        var request = new LoginRequest("acme", "admin@acme.com", "CorrectPassword123!");

        var result = await _useCase.ExecuteAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("fake_jwt_token");
        result.TenantIdentifier.Should().Be("acme");
        result.Email.Should().Be("admin@acme.com");
        _tenantServiceMock.Verify(s => s.SetTenant(tenant.Id, tenant.Identifier), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownTenant_ShouldThrowDomainException()
    {
        _tenantRepoMock.Setup(r => r.GetByIdentifierAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        var request = new LoginRequest("unknown", "admin@unknown.com", "Password123!");

        var action = () => _useCase.ExecuteAsync(request);

        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid tenant or tenant is inactive.");
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_ShouldThrowDomainException()
    {
        var tenant = new Tenant("Acme Corp", "acme");
        var user = new User(tenant.Id, "admin@acme.com", "hashed_password", "Admin User", "Admin");

        _tenantRepoMock.Setup(r => r.GetByIdentifierAsync("acme", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _userRepoMock.Setup(r => r.GetByEmailAsync("admin@acme.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(h => h.Verify("WrongPassword", "hashed_password"))
            .Returns(false);

        var request = new LoginRequest("acme", "admin@acme.com", "WrongPassword");

        var action = () => _useCase.ExecuteAsync(request);

        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid credentials.");
    }
}
