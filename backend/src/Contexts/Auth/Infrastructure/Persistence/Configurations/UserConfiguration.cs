using LabNet.Auth.Domain;
using LabNet.Shared.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabNet.Auth.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly ICurrentTenantService _currentTenantService;

    public UserConfiguration(ICurrentTenantService currentTenantService)
    {
        _currentTenantService = currentTenantService;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).IsRequired().HasMaxLength(50);
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

        builder.HasQueryFilter(u => !_currentTenantService.HasTenant || u.TenantId == _currentTenantService.TenantId);
    }
}
