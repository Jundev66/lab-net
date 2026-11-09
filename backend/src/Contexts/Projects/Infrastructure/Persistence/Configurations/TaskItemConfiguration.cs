using LabNet.Projects.Domain;
using LabNet.Shared.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabNet.Projects.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    private readonly ICurrentTenantService _currentTenantService;

    public TaskItemConfiguration(ICurrentTenantService currentTenantService)
    {
        _currentTenantService = currentTenantService;
    }

    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(250);
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Priority).HasConversion<int>();

        builder.HasQueryFilter(t => !_currentTenantService.HasTenant || t.TenantId == _currentTenantService.TenantId);
    }
}
