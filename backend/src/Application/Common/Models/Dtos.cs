namespace Application.Common.Models;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt);

public sealed record TaskItemDto(
    Guid Id,
    string Title,
    string? Description,
    Domain.Enums.TaskStatus Status,
    DateTime? DueDate,
    Guid ProjectId);
