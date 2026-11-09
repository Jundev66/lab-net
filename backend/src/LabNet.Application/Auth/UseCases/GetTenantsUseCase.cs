using LabNet.Application.Auth.DTOs;
using LabNet.Domain.Interfaces;

namespace LabNet.Application.Auth.UseCases;

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
