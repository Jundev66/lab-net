using LabNet.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace LabNet.Auth.Infrastructure.Persistence;

public interface IAuthDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class TenantRepository : ITenantRepository
{
    private readonly IAuthDbContext _context;

    public TenantRepository(IAuthDbContext context)
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

public class UserRepository : IUserRepository
{
    private readonly IAuthDbContext _context;

    public UserRepository(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
    }
}
