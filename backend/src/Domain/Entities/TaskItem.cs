using Domain.Enums;
using Domain.Exceptions;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Domain.Entities;

public class TaskItem
{
    private TaskItem()
    {
    }

    public TaskItem(
        Guid projectId,
        string title,
        string? description,
        DateTime? dueDate,
        TaskStatus status = TaskStatus.ToDo)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        UpdateDetails(title, description, dueDate);
        ChangeStatus(status);
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project? Project { get; private set; }

    public void UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Task title is required.");
        }

        if (title.Length > 200)
        {
            throw new ValidationException("Task title must be 200 characters or fewer.");
        }

        if (description is { Length: > 2000 })
        {
            throw new ValidationException("Task description must be 2000 characters or fewer.");
        }

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DueDate = dueDate;
    }

    public void ChangeStatus(TaskStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ValidationException("Invalid task status.");
        }

        Status = status;
    }
}
