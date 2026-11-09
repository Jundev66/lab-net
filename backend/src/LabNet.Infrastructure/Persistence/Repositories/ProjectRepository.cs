using LabNet.Domain.Entities;
using LabNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LabNet.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Projects
            .Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Project project, CancellationToken ct = default)
    {
        await _context.Projects.AddAsync(project, ct);
    }

    public Task DeleteAsync(Project project, CancellationToken ct = default)
    {
        _context.Projects.Remove(project);
        return Task.CompletedTask;
    }
}
