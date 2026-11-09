using LabNet.Application.Auth.UseCases;
using LabNet.Application.Projects.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace LabNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<GetTenantsUseCase>();
        services.AddScoped<GetProjectsUseCase>();
        services.AddScoped<GetProjectByIdUseCase>();
        services.AddScoped<CreateProjectUseCase>();
        services.AddScoped<AddTaskUseCase>();
        services.AddScoped<ToggleTaskUseCase>();

        return services;
    }
}
