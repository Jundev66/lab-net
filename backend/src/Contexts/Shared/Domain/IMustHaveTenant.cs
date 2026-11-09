namespace LabNet.Shared.Domain;

public interface IMustHaveTenant
{
    Guid TenantId { get; set; }
}
