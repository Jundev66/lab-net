using LabNet.Domain.Entities;

namespace LabNet.Domain.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Tenant?> GetByIdentifierAsync(string identifier, CancellationToken ct = default);
    Task<IReadOnlyList<Tenant>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
}
