using LabNet.Domain.Common;
using LabNet.Domain.Entities;
using LabNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabNet.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
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

        // Tenant Configuration
        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
            builder.Property(t => t.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(t => t.Identifier).IsUnique();
        });

        // User Configuration
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Role).IsRequired().HasMaxLength(50);
            builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

            // Multitenancy query filter
            builder.HasQueryFilter(u => !_currentTenantService.HasTenant || u.TenantId == _currentTenantService.TenantId);
        });

        // Project Configuration
        modelBuilder.Entity<Project>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Status).HasConversion<int>();

            builder.HasMany(p => p.Tasks)
                .WithOne()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Multitenancy query filter
            builder.HasQueryFilter(p => !_currentTenantService.HasTenant || p.TenantId == _currentTenantService.TenantId);
        });

        // TaskItem Configuration
        modelBuilder.Entity<TaskItem>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(250);
            builder.Property(t => t.Description).HasMaxLength(2000);
            builder.Property(t => t.Priority).HasConversion<int>();

            // Multitenancy query filter
            builder.HasQueryFilter(t => !_currentTenantService.HasTenant || t.TenantId == _currentTenantService.TenantId);
        });
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
