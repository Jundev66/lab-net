using LabNet.Application.Auth.DTOs;
using LabNet.Application.Common.Interfaces;
using LabNet.Domain.Common.Exceptions;
using LabNet.Domain.Interfaces;

namespace LabNet.Application.Auth.UseCases;

public class LoginUseCase
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentTenantService _currentTenantService;

    public LoginUseCase(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ICurrentTenantService currentTenantService)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentTenantService = currentTenantService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.TenantIdentifier))
            throw new DomainException("Tenant identifier is required.");

        var tenant = await _tenantRepository.GetByIdentifierAsync(request.TenantIdentifier, ct);
        if (tenant is null || !tenant.IsActive)
            throw new DomainException("Invalid tenant or tenant is inactive.");

        // Set the tenant context so the user query can resolve within the tenant scope
        _currentTenantService.SetTenant(tenant.Id, tenant.Identifier);

        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            throw new DomainException("Invalid credentials.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid credentials.");

        var token = _jwtTokenGenerator.GenerateToken(user, tenant);

        return new LoginResponse(
            Token: token,
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            TenantId: tenant.Id,
            TenantIdentifier: tenant.Identifier,
            TenantName: tenant.Name
        );
    }
}
