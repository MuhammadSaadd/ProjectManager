using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Enums.TaskStatus;

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

public sealed class TaskRepository(AppDbContext dbContext) : ITaskRepository
{
    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> GetByStatusAsync(
        TaskStatus status,
        CancellationToken cancellationToken = default) =>
        await dbContext.Tasks
            .AsNoTracking()
            .Where(x => x.Status == status)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default) =>
        await dbContext.Tasks.AddAsync(task, cancellationToken);

    public void Remove(TaskItem task) => dbContext.Tasks.Remove(task);
}
