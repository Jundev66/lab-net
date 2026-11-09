using LabNet.Auth.Application.Interfaces;
using LabNet.Auth.Application.UseCases;
using LabNet.Auth.Domain;
using LabNet.Auth.Infrastructure.Persistence;
using LabNet.Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabNet.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthContext(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<LoginUseCase>();
        services.AddScoped<GetTenantsUseCase>();

        return services;
    }
}
