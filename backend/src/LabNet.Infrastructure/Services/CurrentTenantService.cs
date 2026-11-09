using LabNet.Domain.Interfaces;

namespace LabNet.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid? TenantId { get; private set; }
    public string? TenantIdentifier { get; private set; }

    public void SetTenant(Guid tenantId, string? identifier = null)
    {
        TenantId = tenantId;
        if (!string.IsNullOrWhiteSpace(identifier))
        {
            TenantIdentifier = identifier;
        }
    }
}
