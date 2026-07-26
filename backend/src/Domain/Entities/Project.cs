using Domain.Exceptions;

namespace Domain.Entities;

public class Project
{
    private readonly List<TaskItem> _tasks = [];

    private Project()
    {
    }

    public Project(string name, string? description)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdateDetails(name, description);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Project name is required.");
        }

        if (name.Length > 200)
        {
            throw new ValidationException("Project name must be 200 characters or fewer.");
        }

        if (description is { Length: > 2000 })
        {
            throw new ValidationException("Project description must be 2000 characters or fewer.");
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
