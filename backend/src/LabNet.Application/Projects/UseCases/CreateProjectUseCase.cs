using LabNet.Application.Projects.DTOs;
using LabNet.Domain.Common.Exceptions;
using LabNet.Domain.Entities;
using LabNet.Domain.Interfaces;

namespace LabNet.Application.Projects.UseCases;

public class CreateProjectUseCase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantService _currentTenantService;

    public CreateProjectUseCase(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ICurrentTenantService currentTenantService)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _currentTenantService = currentTenantService;
    }

    public async Task<ProjectResponse> ExecuteAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        if (!_currentTenantService.HasTenant)
            throw new DomainException("Tenant context is missing.");

        var project = new Project(_currentTenantService.TenantId!.Value, request.Name, request.Description);
        await _projectRepository.AddAsync(project, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.Status,
            project.CreatedAtUtc,
            Array.Empty<TaskResponse>()
        );
    }
}
