using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.Commands;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskStatus Status = TaskStatus.ToDo) : IRequest<TaskItemDto>;

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
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTaskCommand, TaskItemDto>
{
    public async Task<TaskItemDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        _ = await projects.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        var task = new TaskItem(
            request.ProjectId,
            request.Title,
            request.Description,
            request.DueDate,
            request.Status);

        await tasks.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return task.ToDto();
    }
}
