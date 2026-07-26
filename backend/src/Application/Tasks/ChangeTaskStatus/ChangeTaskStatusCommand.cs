using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.ChangeTaskStatus;

public sealed record ChangeTaskStatusCommand(Guid Id, TaskStatus Status) : IRequest<Result<TaskItemDto>>;

public sealed class ChangeTaskStatusCommandValidator : AbstractValidator<ChangeTaskStatusCommand>
{
    public ChangeTaskStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class ChangeTaskStatusCommandHandler(
    ITaskRepository tasks,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeTaskStatusCommand, Result<TaskItemDto>>
{
    public async Task<Result<TaskItemDto>> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Failure<TaskItemDto>(Error.NotFound(nameof(Domain.Entities.TaskItem), request.Id));

        var statusResult = task.ChangeStatus(request.Status);
        if (statusResult.IsFailure)
            return Result.Failure<TaskItemDto>(statusResult.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(task.ToDto());
    }
}
