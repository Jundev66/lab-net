using LabNet.Domain.Entities;
using LabNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabNet.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<Tenant?> GetByIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        var normalized = identifier.Trim().ToLowerInvariant();
        return await _context.Tenants.FirstOrDefaultAsync(t => t.Identifier == normalized, ct);
    }

    public async Task<IReadOnlyList<Tenant>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.Tenants.Where(t => t.IsActive).OrderBy(t => t.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
    {
        await _context.Tenants.AddAsync(tenant, ct);
    }
}
