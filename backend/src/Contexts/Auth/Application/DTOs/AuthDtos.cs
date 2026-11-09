namespace LabNet.Auth.Application.DTOs;

public record LoginRequest(string TenantIdentifier, string Email, string Password);

public record LoginResponse(
    string Token,
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    Guid TenantId,
    string TenantIdentifier,
    string TenantName
);

public record TenantDto(Guid Id, string Name, string Identifier);
