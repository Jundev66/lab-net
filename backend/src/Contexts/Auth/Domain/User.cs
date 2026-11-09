using LabNet.Shared.Domain;
using LabNet.Shared.Domain.Exceptions;

namespace LabNet.Auth.Domain;

public class User : Entity, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Member";

    private User() { }

    public User(Guid tenantId, string email, string passwordHash, string fullName, string role = "Member")
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID cannot be empty.");

        TenantId = tenantId;
        SetEmail(email);
        SetPasswordHash(passwordHash);
        SetFullName(fullName);
        SetRole(role);
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("A valid email address is required.");
        Email = email.Trim().ToLowerInvariant();
        MarkUpdated();
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty.");
        PasswordHash = passwordHash;
        MarkUpdated();
    }

    public void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.");
        FullName = fullName.Trim();
        MarkUpdated();
    }

    public void SetRole(string role)
    {
        var normalized = role?.Trim();
        if (normalized != "Admin" && normalized != "Member")
            throw new DomainException("Invalid user role. Allowed: Admin, Member.");
        Role = normalized;
        MarkUpdated();
    }
}
