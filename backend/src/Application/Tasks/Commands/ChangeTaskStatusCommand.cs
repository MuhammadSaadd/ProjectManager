using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.Commands;

public sealed record ChangeTaskStatusCommand(Guid Id, TaskStatus Status) : IRequest<TaskItemDto>;

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
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeTaskStatusCommand, TaskItemDto>
{
    public async Task<TaskItemDto> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        task.ChangeStatus(request.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return task.ToDto();
    }
}
