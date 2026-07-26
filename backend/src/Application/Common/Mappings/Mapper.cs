using Domain.Entities;
using Application.Common.Models;

namespace Application.Common.Mappings;

public static class Mapper
{
    public static ProjectDto ToDto(this Project project) =>
        new(project.Id, project.Name, project.Description, project.CreatedAt);

    public static TaskItemDto ToDto(this TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.Status, task.DueDate, task.ProjectId);
}
