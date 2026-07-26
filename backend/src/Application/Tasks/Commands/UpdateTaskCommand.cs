using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.Commands;

public sealed record UpdateTaskCommand(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskStatus Status) : IRequest<TaskItemDto>;

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
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTaskCommand, TaskItemDto>
{
    public async Task<TaskItemDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        task.UpdateDetails(request.Title, request.Description, request.DueDate);
        task.ChangeStatus(request.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return task.ToDto();
    }
}
