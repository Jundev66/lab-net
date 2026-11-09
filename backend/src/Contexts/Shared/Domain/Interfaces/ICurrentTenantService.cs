namespace LabNet.Shared.Domain.Interfaces;

public interface ICurrentTenantService
{
    Guid? TenantId { get; }
    string? TenantIdentifier { get; }
    bool HasTenant => TenantId.HasValue && TenantId.Value != Guid.Empty;
    void SetTenant(Guid tenantId, string? identifier = null);
}
