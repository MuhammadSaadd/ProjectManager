using Application.Common.Interfaces;
using Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Tasks.Commands;

public sealed record DeleteTaskCommand(Guid Id) : IRequest;

public sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteTaskCommandHandler(
    ITaskRepository tasks,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        tasks.Remove(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
