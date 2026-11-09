using LabNet.Shared.Domain;
using LabNet.Shared.Domain.Exceptions;

namespace LabNet.Auth.Domain;

public class Tenant : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Identifier { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private Tenant() { }

    public Tenant(string name, string identifier)
    {
        SetName(name);
        SetIdentifier(identifier);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tenant name cannot be empty.");
        Name = name.Trim();
        MarkUpdated();
    }

    public void SetIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new DomainException("Tenant identifier cannot be empty.");
        Identifier = identifier.Trim().ToLowerInvariant();
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }
}
