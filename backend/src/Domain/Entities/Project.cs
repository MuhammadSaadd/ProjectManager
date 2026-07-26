using Domain.Primitives;

namespace Domain.Entities;

public class Project
{
    private readonly List<TaskItem> _tasks = [];

    private Project()
    {
    }

    private Project(string name, string? description)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = name;
        Description = description;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public static Result<Project> Create(string name, string? description)
    {
        var validation = Validate(name, description);
        if (validation.IsFailure)
            return Result.Failure<Project>(validation.Error!);

        return Result.Success(new Project(name.Trim(), string.IsNullOrWhiteSpace(description) ? null : description.Trim()));
    }

    public Result UpdateDetails(string name, string? description)
    {
        var validation = Validate(name, description);
        if (validation.IsFailure)
            return validation;

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        return Result.Success();
    }

    private static Result Validate(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Project name is required."));

        if (name.Length > 200)
            return Result.Failure(Error.Validation("Project name must be 200 characters or fewer."));

        if (description is { Length: > 2000 })
            return Result.Failure(Error.Validation("Project description must be 2000 characters or fewer."));

        return Result.Success();
    }
}
