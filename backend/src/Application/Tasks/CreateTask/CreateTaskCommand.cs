using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskStatus Status = TaskStatus.ToDo) : IRequest<Result<TaskItemDto>>;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class CreateTaskCommandHandler(
    IProjectRepository projects,
    ITaskRepository tasks,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTaskCommand, Result<TaskItemDto>>
{
    public async Task<Result<TaskItemDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<TaskItemDto>(Error.NotFound(nameof(Project), request.ProjectId));

        var taskResult = TaskItem.Create(request.ProjectId, request.Title, request.Description, request.DueDate, request.Status);
        if (taskResult.IsFailure)
            return Result.Failure<TaskItemDto>(taskResult.Error!);

        var task = taskResult.Value;
        await tasks.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(task.ToDto());
    }
}
