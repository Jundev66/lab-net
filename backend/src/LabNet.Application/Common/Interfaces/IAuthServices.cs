using LabNet.Domain.Entities;

namespace LabNet.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, Tenant tenant);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
