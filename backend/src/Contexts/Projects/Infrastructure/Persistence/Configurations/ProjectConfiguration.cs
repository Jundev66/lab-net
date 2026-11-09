using LabNet.Projects.Domain;
using LabNet.Shared.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabNet.Projects.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    private readonly ICurrentTenantService _currentTenantService;

    public ProjectConfiguration(ICurrentTenantService currentTenantService)
    {
        _currentTenantService = currentTenantService;
    }

    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Status).HasConversion<int>();

        builder.HasMany(p => p.Tasks)
            .WithOne()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !_currentTenantService.HasTenant || p.TenantId == _currentTenantService.TenantId);
    }
}
