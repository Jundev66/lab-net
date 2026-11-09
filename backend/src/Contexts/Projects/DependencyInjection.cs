using LabNet.Projects.Application.UseCases;
using LabNet.Projects.Domain;
using LabNet.Projects.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LabNet.Projects;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsContext(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();

        services.AddScoped<GetProjectsUseCase>();
        services.AddScoped<GetProjectByIdUseCase>();
        services.AddScoped<CreateProjectUseCase>();
        services.AddScoped<AddTaskUseCase>();
        services.AddScoped<ToggleTaskUseCase>();

        return services;
    }
}
