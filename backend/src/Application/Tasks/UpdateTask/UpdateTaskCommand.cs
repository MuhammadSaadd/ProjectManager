using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskStatus Status) : IRequest<Result<TaskItemDto>>;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpdateTaskCommandHandler(
    ITaskRepository tasks,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTaskCommand, Result<TaskItemDto>>
{
    public async Task<Result<TaskItemDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Failure<TaskItemDto>(Error.NotFound(nameof(Domain.Entities.TaskItem), request.Id));

        var updateResult = task.UpdateDetails(request.Title, request.Description, request.DueDate);
        if (updateResult.IsFailure)
            return Result.Failure<TaskItemDto>(updateResult.Error!);

        var statusResult = task.ChangeStatus(request.Status);
        if (statusResult.IsFailure)
            return Result.Failure<TaskItemDto>(statusResult.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(task.ToDto());
    }
}
