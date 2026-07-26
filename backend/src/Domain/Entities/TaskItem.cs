using Domain.Enums;
using Domain.Primitives;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Domain.Entities;

public class TaskItem
{
    private TaskItem()
    {
    }

    private TaskItem(Guid projectId, string title, string? description, DateTime? dueDate, TaskStatus status)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project? Project { get; private set; }

    public static Result<TaskItem> Create(Guid projectId, string title, string? description, DateTime? dueDate, TaskStatus status = TaskStatus.ToDo)
    {
        var validation = Validate(title, description);
        if (validation.IsFailure)
            return Result.Failure<TaskItem>(validation.Error!);

        if (!Enum.IsDefined(status))
            return Result.Failure<TaskItem>(Error.Validation("Invalid task status."));

        return Result.Success(new TaskItem(
            projectId,
            title.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            dueDate,
            status));
    }

    public Result UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        var validation = Validate(title, description);
        if (validation.IsFailure)
            return validation;

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DueDate = dueDate;
        return Result.Success();
    }

    public Result ChangeStatus(TaskStatus status)
    {
        if (!Enum.IsDefined(status))
            return Result.Failure(Error.Validation("Invalid task status."));

        Status = status;
        return Result.Success();
    }

    private static Result Validate(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("Task title is required."));

        if (title.Length > 200)
            return Result.Failure(Error.Validation("Task title must be 200 characters or fewer."));

        if (description is { Length: > 2000 })
            return Result.Failure(Error.Validation("Task description must be 2000 characters or fewer."));

        return Result.Success();
    }
}
