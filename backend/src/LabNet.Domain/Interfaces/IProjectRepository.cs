using LabNet.Domain.Entities;

namespace LabNet.Domain.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
    Task DeleteAsync(Project project, CancellationToken ct = default);
}
