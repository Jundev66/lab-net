using LabNet.Auth.Domain;

namespace LabNet.Auth.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, Tenant tenant);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
