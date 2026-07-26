using Domain.Repositories;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Tasks.GetTasksByStatus;

public sealed record GetTasksByStatusQuery(TaskStatus Status) : IRequest<Result<IReadOnlyList<TaskItemDto>>>;

public sealed class GetTasksByStatusQueryValidator : AbstractValidator<GetTasksByStatusQuery>
{
    public GetTasksByStatusQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class GetTasksByStatusQueryHandler(ITaskRepository tasks)
    : IRequestHandler<GetTasksByStatusQuery, Result<IReadOnlyList<TaskItemDto>>>
{
    public async Task<Result<IReadOnlyList<TaskItemDto>>> Handle(
        GetTasksByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var items = await tasks.GetByStatusAsync(request.Status, cancellationToken);
        return Result.Success(items.Select(t => t.ToDto()).ToList() as IReadOnlyList<TaskItemDto>);
    }
}
