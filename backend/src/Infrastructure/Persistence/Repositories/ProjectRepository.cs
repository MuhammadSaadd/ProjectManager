using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(AppDbContext dbContext) : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
        await dbContext.Projects.AddAsync(project, cancellationToken);

    public void Remove(Project project) => dbContext.Projects.Remove(project);
}
