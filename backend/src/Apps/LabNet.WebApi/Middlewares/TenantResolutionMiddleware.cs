using LabNet.Auth.Domain;
using LabNet.Shared.Domain.Interfaces;

namespace LabNet.WebApi.Middlewares;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTenantService, ITenantRepository tenantRepository)
    {
        var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
        var tenantIdentifierClaim = context.User.FindFirst("tenant_identifier")?.Value;

        if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var parsedTenantId))
        {
            currentTenantService.SetTenant(parsedTenantId, tenantIdentifierClaim);
        }
        else if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenantId) && Guid.TryParse(headerTenantId, out var parsedHeaderId))
        {
            currentTenantService.SetTenant(parsedHeaderId);
        }
        else if (context.Request.Headers.TryGetValue("X-Tenant-Identifier", out var headerTenantIdentifier))
        {
            var tenant = await tenantRepository.GetByIdentifierAsync(headerTenantIdentifier!);
            if (tenant is not null && tenant.IsActive)
            {
                currentTenantService.SetTenant(tenant.Id, tenant.Identifier);
            }
        }

        await _next(context);
    }
}
