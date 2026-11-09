using LabNet.Auth.Application.DTOs;
using LabNet.Auth.Application.Interfaces;
using LabNet.Auth.Domain;
using LabNet.Shared.Domain.Exceptions;
using LabNet.Shared.Domain.Interfaces;

namespace LabNet.Auth.Application.UseCases;

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

public class GetTenantsUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantsUseCase(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<IReadOnlyList<TenantDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var tenants = await _tenantRepository.GetAllActiveAsync(ct);
        return tenants.Select(t => new TenantDto(t.Id, t.Name, t.Identifier)).ToList();
    }
}
