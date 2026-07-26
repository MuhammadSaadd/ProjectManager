using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    void Remove(Project project);
}

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByStatusAsync(Domain.Enums.TaskStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    void Remove(TaskItem task);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
