using LabNet.Auth.Domain;
using LabNet.Auth.Infrastructure.Persistence;
using LabNet.Auth.Infrastructure.Persistence.Configurations;
using LabNet.Projects.Domain;
using LabNet.Projects.Infrastructure.Persistence;
using LabNet.Projects.Infrastructure.Persistence.Configurations;
using LabNet.Shared.Domain;
using LabNet.Shared.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabNet.WebApi.Persistence;

public class AppDbContext : DbContext, IUnitOfWork, IAuthDbContext, IProjectsDbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentTenantService currentTenantService) : base(options)
    {
        _currentTenantService = currentTenantService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Bounded Context DB Configurations
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration(_currentTenantService));
        modelBuilder.ApplyConfiguration(new ProjectConfiguration(_currentTenantService));
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration(_currentTenantService));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTenantService.HasTenant)
        {
            var currentTenantId = _currentTenantService.TenantId!.Value;
            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
            {
                if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = currentTenantId;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
