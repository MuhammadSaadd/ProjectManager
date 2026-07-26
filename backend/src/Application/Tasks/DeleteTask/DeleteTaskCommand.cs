using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Tasks.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteTaskCommandHandler(
    ITaskRepository tasks,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NotFound(nameof(Domain.Entities.TaskItem), request.Id));

        tasks.Remove(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
