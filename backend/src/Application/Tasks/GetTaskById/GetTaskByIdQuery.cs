using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.Tasks.GetTaskById;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskItemDto>>;

public sealed class GetTaskByIdQueryValidator : AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class GetTaskByIdQueryHandler(ITaskRepository tasks)
    : IRequestHandler<GetTaskByIdQuery, Result<TaskItemDto>>
{
    public async Task<Result<TaskItemDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Failure<TaskItemDto>(Error.NotFound(nameof(Domain.Entities.TaskItem), request.Id));

        return Result.Success(task.ToDto());
    }
}
